import assert from "node:assert/strict"
import { readFileSync } from "node:fs"
import { test } from "node:test"
import * as Blockly from "blockly"
import { DataFactory, Store } from "n3"
import { isomorphic } from "rdf-isomorphic"
import dataset_ from "@rdfjs/dataset"
import type { DatasetCore, Quad, Term } from "@rdfjs/types"
import { defineBlocks } from "../blocks.ts"
import { buildSchema } from "../schema/mod.ts"
import { datasetToState, parse, quadsToTurtle, stateToQuads } from "../rdf.ts"
import { RDF } from "../vocabulary/mod.ts"
import { ManifestDataset } from "../wrapping/ManifestDataset.ts"

const read = (path: string) => readFileSync(new URL(path, import.meta.url), "utf8")
const schema = buildSchema(dataset_.dataset([...parse(read("../../../Model/Resources/vocabulary.ttl")), ...parse(read("../../shapes.ttl"))]))
const files = ["new", "example", "touchstone", "oldmanifest", "fromSpec", "oldmanifest/did_key"]
const load = (file: string) => parse(read(`../../../Model/Resources/${file}.ttl`))

function rootOf(dataset: DatasetCore) {
    return new ManifestDataset(dataset, DataFactory).root as unknown as Term
}

const indexed = (dataset: DatasetCore) => new Store([...dataset])

function reachable(dataset: DatasetCore, root: Term) {
    dataset = indexed(dataset)
    const result: Quad[] = []
    const seen = new Set<string>()
    const queue = [root]
    for (let node = queue.shift(); node; node = queue.shift()) {
        const key = node.termType + node.value
        if (seen.has(key)) {
            continue
        }
        seen.add(key)
        for (const q of dataset.match(node)) {
            result.push(q)
            if (q.object.termType !== "Literal") {
                queue.push(q.object)
            }
        }
    }
    return result
}

function sharesNodes(quads: Quad[]) {
    const incoming = new Set<string>()
    return quads.some(q => {
        if (q.object.termType !== "BlankNode") {
            return false
        }
        const known = incoming.has(q.object.value)
        incoming.add(q.object.value)
        return known
    })
}

function tree(dataset: DatasetCore, node: Term): unknown {
    return expand(indexed(dataset), node)
}

function expand(dataset: DatasetCore, node: Term): unknown {
    if (node.termType === "Literal") {
        return `"${node.value}"^^${node.datatype.value}`
    }
    if (node.termType === "NamedNode" && node.value === RDF.nil) {
        return []
    }
    const properties = new Map<string, unknown[]>()
    for (const q of dataset.match(node)) {
        properties.set(q.predicate.value, [...(properties.get(q.predicate.value) ?? []), expand(dataset, q.object)])
    }
    if (properties.has(RDF.first)) {
        const list: unknown[] = []
        for (let current: Term | undefined = node; current && !(current.termType === "NamedNode" && current.value === RDF.nil); ) {
            list.push(expand(dataset, [...dataset.match(current, DataFactory.namedNode(RDF.first))][0].object))
            current = [...dataset.match(current, DataFactory.namedNode(RDF.rest))][0]?.object
        }
        return list
    }
    const sorted = [...properties].map(([predicate, values]) => [predicate, values.map(value => JSON.stringify(value)).sort().map(value => JSON.parse(value))] as const)
    return { ...(node.termType === "NamedNode" && { "@id": node.value }), ...Object.fromEntries(sorted.sort(([a], [b]) => a.localeCompare(b))) }
}

function roundTrip(file: string) {
    const dataset = load(file)
    const root = rootOf(dataset)
    const original = reachable(dataset, root)
    const generated = dataset_.dataset(stateToQuads(datasetToState(dataset, schema), schema))
    return { dataset, root, original, generated }
}

for (const file of files) {
    test(`round trip ${file}`, () => {
        const { dataset, root, original, generated } = roundTrip(file)
        const shared = sharesNodes(original)
        const strict = !shared && isomorphic(original, [...generated])
        if (!shared) {
            assert.ok(strict)
        }
        console.log(`${file}: ${original.length} triples, strictly isomorphic: ${strict}, shared nodes: ${shared}`)
        assert.equal(JSON.stringify(tree(generated, rootOf(generated)), null, 1), JSON.stringify(tree(dataset, root), null, 1))
    })
}

test("turtle round trip", () => {
    const { original, generated } = roundTrip("new")
    assert.ok(isomorphic(original, [...parse(quadsToTurtle([...generated]))]))
})

for (const file of ["example", "touchstone"]) {
    test(`turtle round trip ${file}`, () => {
        const { generated } = roundTrip(file)
        const turtle = quadsToTurtle([...generated])
        const parsed = dataset_.dataset([...parse(turtle)])
        if (file === "example") {
            assert.ok(isomorphic([...generated], [...parsed]))
        }
        assert.equal(JSON.stringify(tree(parsed, rootOf(parsed))), JSON.stringify(tree(generated, rootOf(generated))))
        assert.ok(turtle.includes("["))
    })
}

test("lists and datatypes", () => {
    const { generated } = roundTrip("example")
    const turtle = quadsToTurtle([...generated])
    assert.ok(turtle.includes("rdf:first") || turtle.includes("("))
    assert.ok(roundTrip("touchstone").generated.size > 0)
})

for (const file of ["example", "touchstone"]) {
    test(`headless workspace round trip ${file}`, () => {
        const dataset = load(file)
        const expected = tree(dataset, rootOf(dataset))
        defineBlocks(schema)
        const workspace = new Blockly.Workspace()
        Blockly.serialization.workspaces.load(datasetToState(dataset, schema), workspace)
        const saved = Blockly.serialization.workspaces.save(workspace)
        const generated = dataset_.dataset(stateToQuads(saved, schema))
        assert.deepEqual(tree(generated, rootOf(generated)), expected)
        workspace.dispose()
    })
}

function variableOptions(workspace: Blockly.Workspace, block: Blockly.Block) {
    const field = block.getField("VARIABLE") as Blockly.FieldDropdown
    return field.getOptions(false).map(([, value]) => value)
}

test("variable scope", () => {
    defineBlocks(schema)
    const workspace = new Blockly.Workspace()
    Blockly.serialization.workspaces.load(datasetToState(load("example"), schema), workspace)
    const vars = workspace.getAllBlocks().filter(block => block.type === "lws_variableexpression")
    const byValue = (value: string) => vars.find(block => block.getFieldValue("VARIABLE") === value)!
    assert.deepEqual(variableOptions(workspace, byValue("baseUri")), ["baseUri"])
    assert.deepEqual(variableOptions(workspace, byValue("storageRoot")), ["baseUri", "storageRoot"])
    assert.deepEqual(variableOptions(workspace, byValue("newResource")), ["baseUri", "storageRoot", "newResource"])
    assert.deepEqual(variableOptions(workspace, byValue("status")), ["baseUri", "storageRoot", "newResource", "status"])
    byValue("storageRoot").setFieldValue("early", "VARIABLE")
    assert.deepEqual(variableOptions(workspace, vars.find(block => block.getFieldValue("VARIABLE") === "early")!), ["baseUri", "storageRoot", "early"])
    byValue("status").setFieldValue("unknown", "VARIABLE")
    assert.equal(vars.some(block => block.getFieldValue("VARIABLE") === "unknown"), true)
    assert.deepEqual(variableOptions(workspace, vars.find(block => block.getFieldValue("VARIABLE") === "unknown")!), ["baseUri", "storageRoot", "newResource", "status", "unknown"])
    workspace.dispose()
})
