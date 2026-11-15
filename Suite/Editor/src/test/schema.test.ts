import assert from "node:assert/strict"
import { readFileSync } from "node:fs"
import { test } from "node:test"
import * as Blockly from "blockly"
import { Parser } from "n3"
import dataset from "@rdfjs/dataset"
import { buildSchema } from "../schema/mod.ts"
import { choose, matches } from "../schema/mod.ts"
import { blockDefinitions, defineBlocks, toolbox } from "../blocks.ts"
import { addEntries } from "../components/AddMenu.ts"
import type { BlockKind, Member, NodeFacts } from "../schema/mod.ts"

const read = (path: string) => readFileSync(new URL(path, import.meta.url), "utf8")
const quads = [
    ...new Parser().parse(read("../../../Model/Resources/vocabulary.ttl")),
    ...new Parser().parse(read("../../shapes.ttl")),
]
const schema = buildSchema(dataset.dataset(quads))
const E = "http://example.com/"
const XSD = "http://www.w3.org/2001/XMLSchema#"
const RDF_TYPE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type"

const byType = (type: string) => {
    const block = schema.blocks.find(b => b.type === type)
    assert.ok(block, type)
    return block
}
const member = (block: BlockKind, name: string) => block.members.find(m => m.name === name) as Member
const names = (members: Member[]) => members.map(m => m.name)
const local = (iri: string) => iri.slice(E.length)

test("root and counts", () => {
    assert.equal(schema.root, E + "Manifest")
    assert.equal(byType("lws_manifest").connection, "root")
    assert.equal(new Set(schema.blocks.map(b => b.type)).size, schema.blocks.length)
    assert.equal(schema.blocks.length, 27)
    assert.deepEqual(schema.abstracts.map(a => local(a.class)).sort(), ["Assertion", "Expression", "Extractor"])
})

test("abstract alternatives keep dispatch order", () => {
    const alternatives = (name: string) => schema.abstracts.find(a => a.class === E + name)!.alternatives.map(local)
    assert.deepEqual(alternatives("Expression"), ["ConstantExpression", "VariableExpression", "ResolveExpression", "SingleExpression", "CountExpression", "FormExpression"])
    assert.deepEqual(alternatives("Extractor"), ["BodyExtractor", "GraphExtractor", "HeaderExtractor", "LinkExtractor", "JsonPathExtractor", "StatusCodeExtractor"])
    assert.deepEqual(alternatives("Assertion"), ["EqualityAssertion", "RegexAssertion", "ContainsAssertion", "GreaterThanAssertion", "IsomorphicAssertion", "NotAssertion"])
})

test("categories", () => {
    assert.deepEqual(schema.categories.map(c => c.name).sort(), ["Assertion", "Expression", "Extractor", "Structure"])
    const expression = schema.categories.find(c => c.name === "Expression")!
    assert.deepEqual(expression.blocks, ["lws_constantexpression_string", "lws_constantexpression_integer", "lws_variableexpression", "lws_resolveexpression", "lws_singleexpression", "lws_countexpression", "lws_formexpression"])
    assert.equal(expression.hue, 120)
    const structure = schema.categories.find(c => c.name === "Structure")!
    assert.ok(structure.blocks.includes("lws_manifest"))
    assert.ok(structure.blocks.includes("lws_include"))
})

test("lists and sets", () => {
    const testCase = byType("lws_testcase")
    assert.equal(testCase.connection, "statement")
    assert.deepEqual(names(testCase.members), ["TEST_NAME", "SPECIFICATION", "STEPS", "ASSERT"])
    assert.deepEqual(member(testCase, "STEPS"), {
        kind: "statements", name: "STEPS", label: "steps", tooltip: member(testCase, "STEPS").tooltip, predicate: E + "steps",
        target: E + "Step", check: E + "Step", list: true, required: true,
    })
    assert.deepEqual(member(byType("lws_step"), "EXTRACT"), { ...member(byType("lws_step"), "EXTRACT"), list: true, target: E + "Extractor", check: E + "Extractor" })
    assert.deepEqual(member(byType("lws_formexpression"), "FORM"), { ...member(byType("lws_formexpression"), "FORM"), list: true, target: E + "FormField" })
    const tests = member(byType("lws_manifest"), "TEST")
    assert.ok(tests.kind === "statements" && !tests.list && tests.check === E + "TestCase")
    assert.equal(byType("lws_step").connection, "statement")
    assert.equal(byType("lws_requestheader").connection, "statement")
    assert.equal(byType("lws_formfield").connection, "statement")
    assert.equal(byType("lws_request").connection, "value")
})

test("literal sets become item blocks", () => {
    const include = byType("lws_include")
    assert.ok(include.literal)
    assert.equal(include.connection, "statement")
    assert.deepEqual(include.checks, [E + "include"])
    const field = include.members[0]
    assert.ok(field.kind === "field" && field.datatype === XSD + "anyURI")
    assert.equal(member(byType("lws_manifest"), "INCLUDE").kind, "statements")
    assert.ok(byType("lws_specification").literal)
})

test("block kinds expose required predicates and types for matching", () => {
    for (const name of ["Body", "Graph", "StatusCode"]) {
        const block = byType(`lws_${name.toLowerCase()}extractor`)
        assert.deepEqual(block.required, [E + "to"])
        assert.deepEqual(block.types, [`${E}${name}Extractor`])
        assert.deepEqual(names(block.members), ["TO"])
    }
    const header = byType("lws_headerextractor")
    assert.deepEqual(header.required, [E + "header", E + "to"])
    assert.deepEqual(header.types, [])
    assert.deepEqual(names(header.members), ["HEADER", "TO"])
    assert.equal(header.hue, 260)
    assert.deepEqual(header.checks, [E + "HeaderExtractor", E + "Extractor"])
    const resolve = byType("lws_resolveexpression")
    assert.deepEqual(resolve.required, [E + "base", E + "relative"])
    assert.deepEqual(byType("lws_manifest").required, [E + "manifestName"])
})

test("constant has a block per datatype", () => {
    const string = member(byType("lws_constantexpression_string"), "CONSTANT")
    const integer = member(byType("lws_constantexpression_integer"), "CONSTANT")
    assert.ok(string.kind === "field" && string.datatype === XSD + "string")
    assert.ok(integer.kind === "field" && integer.datatype === XSD + "integer")
    const block = byType("lws_constantexpression_integer")
    assert.deepEqual(block.required, [E + "constant"])
    assert.deepEqual(block.checks, [E + "ConstantExpression", E + "Expression"])
    assert.equal(block.hue, 120)
    assert.equal(block.connection, "value")
})

test("assertions inherit that from the value assertion", () => {
    const assertions = schema.blocks.filter(b => b.checks.includes(E + "Assertion"))
    assert.equal(assertions.length, 6)
    assert.ok(!schema.abstracts.some(a => a.class === E + "ValueAssertion"))
    assert.ok(!schema.blocks.some(b => b.class === E + "ValueAssertion"))
    const equality = byType("lws_equalityassertion")
    assert.deepEqual(names(equality.members), ["THAT", "EQUALS"])
    assert.equal(member(equality, "THAT").required, true)
    assert.deepEqual(equality.required, [E + "that", E + "equals"])
    assert.deepEqual(equality.checks, [E + "EqualityAssertion", E + "ValueAssertion", E + "Assertion"])
    assert.equal(equality.hue, 340)
    const not = byType("lws_notassertion")
    assert.deepEqual(names(not.members), ["NOT"])
    assert.deepEqual(not.required, [E + "not"])
    assert.deepEqual(not.checks, [E + "NotAssertion", E + "Assertion"])
})

test("assert and not both take a complete assertion", () => {
    const assertion = member(byType("lws_testcase"), "ASSERT")
    assert.ok(assertion.kind === "value" && assertion.check === E + "Assertion")
    const operand = member(byType("lws_notassertion"), "NOT")
    assert.ok(operand.kind === "value" && operand.check === E + "Assertion")
    assert.equal(byType("lws_notassertion").connection, "value")
})

const facts = (types: string[], ...predicates: string[]): NodeFacts => ({
    types: new Set(types.map(t => E + t)),
    predicates: new Set(predicates.map(p => E + p)),
})

test("matches and choose pick the alternative a node satisfies", () => {
    const resolve = byType("lws_resolveexpression")
    assert.ok(matches(resolve, facts([], "base", "relative")))
    assert.ok(!matches(resolve, facts([], "base")))
    assert.equal(choose(schema, E + "Expression", facts([], "base", "relative"))?.type, "lws_resolveexpression")
    assert.equal(choose(schema, E + "Expression", facts([], "constant"))?.type, "lws_constantexpression_string")
    assert.equal(choose(schema, E + "Expression", facts([], "form"))?.type, "lws_formexpression")
    assert.equal(choose(schema, E + "Expression", facts([], "base")), undefined)
    assert.equal(choose(schema, E + "Extractor", facts(["BodyExtractor"], "to"))?.type, "lws_bodyextractor")
    assert.equal(choose(schema, E + "Extractor", facts(["StatusCodeExtractor"], "to"))?.type, "lws_statuscodeextractor")
    assert.equal(choose(schema, E + "Extractor", facts([], "to", "header"))?.type, "lws_headerextractor")
    assert.equal(choose(schema, E + "Extractor", facts([], "to")), undefined)
    assert.equal(choose(schema, E + "Assertion", facts([], "that", "contains"))?.type, "lws_containsassertion")
    assert.equal(choose(schema, E + "Assertion", facts([], "not"))?.type, "lws_notassertion")
    assert.equal(choose(schema, E + "Assertion", facts([], "equals")), undefined)
})

test("choose classifies the nodes of the manifests", () => {
    const manifest = ["example", "touchstone", "fromSpec", "oldmanifest"].flatMap(name => new Parser().parse(read(`../../../Model/Resources/${name}.ttl`)))
    const bySubject = new Map<string, NodeFacts & { predicates: Set<string>, types: Set<string> }>()
    for (const q of manifest) {
        const key = q.subject.termType + q.subject.value
        const node = bySubject.get(key) ?? { predicates: new Set<string>(), types: new Set<string>() }
        node.predicates.add(q.predicate.value)
        if (q.predicate.value === RDF_TYPE) {
            node.types.add(q.object.value)
        }
        bySubject.set(key, node)
    }
    const classes = new Map<string, number>()
    for (const [predicate, abstract] of [[E + "that", E + "Expression"], [E + "uri", E + "Expression"], [E + "base", E + "Expression"], [E + "relative", E + "Expression"], [E + "assert", E + "Assertion"], [E + "not", E + "Assertion"]]) {
        for (const q of manifest.filter(q => q.predicate.value === predicate)) {
            const node = bySubject.get(q.object.termType + q.object.value)!
            const block = choose(schema, abstract, node)
            assert.ok(block, `${predicate} ${q.object.value}`)
            classes.set(block.class!, (classes.get(block.class!) ?? 0) + 1)
        }
    }
    for (const [key, node] of bySubject) {
        if (node.predicates.has(E + "to")) {
            const block = choose(schema, E + "Extractor", node)
            assert.ok(block, key)
            classes.set(block.class!, (classes.get(block.class!) ?? 0) + 1)
        }
    }
    for (const name of ["ResolveExpression", "ConstantExpression", "VariableExpression", "EqualityAssertion", "NotAssertion", "ContainsAssertion", "HeaderExtractor", "JsonPathExtractor"]) {
        assert.ok(classes.get(E + name), name)
    }
})

test("labels, tooltips, order", () => {
    const request = byType("lws_request")
    assert.equal(request.label, "")
    assert.equal(request.tooltip, "HTTP request.")
    assert.deepEqual(names(request.members), ["METHOD", "URI", "HEADERS", "BODY"])
    assert.deepEqual(request.members.map(m => m.kind), ["field", "value", "statements", "value"])
    assert.equal(member(request, "URI").label, "URI")
    assert.ok(member(request, "BODY").tooltip!.startsWith("Expression evaluating to the content"))
    assert.equal(member(request, "BODY").required, false)
    assert.equal(byType("lws_manifest").hue, 20)
})

test("block definitions", () => {
    const definitions = blockDefinitions(schema) as Record<string, any>[]
    assert.equal(definitions.length, 27)
    assert.equal(new Set(definitions.map(d => d.type)).size, 27)
    defineBlocks(schema)
    for (const definition of definitions) {
        assert.ok(Blockly.Blocks[definition.type], definition.type)
    }
    const by = (type: string) => definitions.find(d => d.type === type)!
    const manifest = by("lws_manifest")
    assert.equal(manifest.output, undefined)
    assert.equal(manifest.previousStatement, undefined)
    assert.equal(manifest.args0[0].name, "MANIFEST_NAME")
    assert.equal(manifest.inputsInline, false)
    const inputs = (type: string) => Object.fromEntries(Object.entries(by(type)).filter(([k]) => k.startsWith("args")).flatMap(([, v]) => v as any[]).map(a => [a.name, a]))
    assert.equal(inputs("lws_request").URI.check, E + "Expression")
    assert.equal(inputs("lws_notassertion").NOT.check, E + "Assertion")
    assert.equal(inputs("lws_testcase").STEPS.check, E + "Step")
    assert.equal(inputs("lws_step").EXTRACT.check, E + "Extractor")
    assert.equal(inputs("lws_constantexpression_integer").CONSTANT.type, "field_number")
    assert.equal(inputs("lws_constantexpression_integer").CONSTANT.precision, 1)
    assert.ok(by("lws_headerextractor").previousStatement.includes(E + "Extractor"))
    assert.ok(by("lws_step").previousStatement.includes(E + "Step"))
    const contents = toolbox(schema).contents
    const names = (kind: string) => contents.find(c => c.name === kind)!.contents.map(c => c.type)
    assert.ok(!names("Structure").includes("lws_manifest"))
    assert.equal(contents.flatMap(c => c.contents).length, 26)
})

test("add menu entries", () => {
    const texts = (type: string) => addEntries(schema, byType(type)).map(entry => entry.text)
    assert.equal(texts("lws_step")[0], "Set request: request")
    assert.ok(texts("lws_step").includes("Add extractor: header"))
    assert.ok(texts("lws_request").includes("Set body: expression: constant expression (string)"))
    assert.ok(texts("lws_request").includes("Add request header"))
    assert.deepEqual(texts("lws_bodyextractor"), [])
})
