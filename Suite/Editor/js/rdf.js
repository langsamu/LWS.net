import { DataFactory, Parser, Writer } from "n3"
import dataset from "@rdfjs/dataset"
import { DatasetWrapper, LiteralAs, LiteralFrom, RequiredFrom, SetFrom, TermWrapper } from "@rdfjs/wrapper"
import { NS, blockTable } from "./blocks.js"

const { namedNode, blankNode, literal, quad } = DataFactory

export function parse(text) {
    return dataset.dataset(new Parser().parse(text))
}

class Manifest extends TermWrapper {
    get manifestName() {
        return RequiredFrom.subjectPredicate(this, NS + "manifestName", LiteralAs.string)
    }

    get include() {
        return SetFrom.subjectPredicate(this, NS + "include", LiteralAs.string, LiteralFrom.anyUriString)
    }
}

class ManifestDataset extends DatasetWrapper {
    get root() {
        const subjects = new Map()
        for (const predicate of ["test", "include"]) {
            for (const q of this.match(undefined, namedNode(NS + predicate))) {
                subjects.set(q.subject.termType + q.subject.value, q.subject)
            }
        }
        if (subjects.size !== 1) {
            throw new Error(`Expected exactly one root manifest, found ${subjects.size}`)
        }
        return new Manifest([...subjects.values()][0], this, DataFactory)
    }
}

export function datasetToState(dataset) {
    const manifest = new ManifestDataset(dataset, DataFactory).root
    let next
    for (const path of [...manifest.include].reverse()) {
        next = { type: "lws_include", fields: { PATH: path }, ...(next && { next: { block: next } }) }
    }
    const block = { type: "lws_manifest", fields: { MANIFEST_NAME: manifest.manifestName } }
    if (next) {
        block.inputs = { INCLUDES: { block: next } }
    }
    return { blocks: { languageVersion: 0, blocks: [block] } }
}

function* chain(first) {
    for (let block = first; block; block = block.next?.block) {
        yield block
    }
}

function walk(block, quads) {
    const rdf = blockTable[block.type].rdf
    const subject = blankNode()
    for (const [name, mapping] of Object.entries(rdf.fields ?? {})) {
        quads.push(quad(subject, namedNode(mapping.predicate), literal(block.fields?.[name] ?? "")))
    }
    for (const [name, mapping] of Object.entries(rdf.statements ?? {})) {
        for (const child of chain(block.inputs?.[name]?.block)) {
            const childRdf = blockTable[child.type].rdf
            if (childRdf.literal) {
                const value = child.fields?.[childRdf.literal.field] ?? ""
                quads.push(quad(subject, namedNode(mapping.predicate), literal(value, namedNode(childRdf.literal.datatype))))
            } else {
                quads.push(quad(subject, namedNode(mapping.predicate), walk(child, quads)))
            }
        }
    }
    return subject
}

export function stateToQuads(state) {
    const quads = []
    for (const block of state.blocks?.blocks ?? []) {
        if (blockTable[block.type]?.rdf.root) {
            walk(block, quads)
        }
    }
    return quads
}

export function quadsToTurtle(quads) {
    const writer = new Writer({ prefixes: { "": NS, xsd: "http://www.w3.org/2001/XMLSchema#" } })
    writer.addQuads(quads)
    let result
    writer.end((error, text) => {
        if (error) {
            throw error
        }
        result = text
    })
    return result
}

export function rootOnlyQuads(dataset) {
    const root = new ManifestDataset(dataset, DataFactory).root
    const keep = [NS + "manifestName", NS + "include"]
    return [...dataset.match(root)].filter(q => keep.includes(q.predicate.value))
}

export function isomorphic(a, b) {
    const key = q => [q.predicate.value, q.object.termType, q.object.value, q.object.datatype?.value ?? ""].join("|")
    const group = quads => {
        if (new Set(quads.map(q => q.subject.termType + q.subject.value)).size > 1) {
            throw new Error("Only single-subject graphs are supported")
        }
        return quads.map(key).sort()
    }
    const left = group(a)
    const right = group(b)
    return left.length === right.length && left.every((value, index) => value === right[index])
}
