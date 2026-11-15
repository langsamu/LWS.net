import { DataFactory, Parser, Writer } from "n3"
import type { DatasetCore, Quad, Term } from "@rdfjs/types"
import dataset from "@rdfjs/dataset"
import { blockTable } from "./blocks.ts"
import { Vocabulary } from "./vocabulary/mod.ts"
import { ManifestDataset } from "./wrapping/ManifestDataset.ts"

const { namedNode, blankNode, literal, quad } = DataFactory

export interface Block {
    type: string
    fields?: Record<string, string>
    inputs?: Record<string, { block?: Block }>
    next?: { block?: Block }
}

export interface State {
    blocks?: { blocks?: Block[] }
}

export function parse(text: string) {
    return dataset.dataset(new Parser().parse(text))
}

export function datasetToState(dataset: DatasetCore) {
    const manifest = new ManifestDataset(dataset, DataFactory).root
    let next: Block | undefined
    for (const path of [...manifest.include].reverse()) {
        next = { type: "lws_include", fields: { PATH: path }, ...(next && { next: { block: next } }) }
    }
    const block: Block = { type: "lws_manifest", fields: { MANIFEST_NAME: manifest.manifestName } }
    if (next) {
        block.inputs = { INCLUDES: { block: next } }
    }
    return { blocks: { languageVersion: 0, blocks: [block] } }
}

function* chain(first: Block | undefined) {
    for (let block = first; block; block = block.next?.block) {
        yield block
    }
}

function walk(block: Block, quads: Quad[]) {
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

export function stateToQuads(state: State) {
    const quads: Quad[] = []
    for (const block of state.blocks?.blocks ?? []) {
        if (blockTable[block.type]?.rdf.root) {
            walk(block, quads)
        }
    }
    return quads
}

export function quadsToTurtle(quads: Quad[]) {
    const writer = new Writer({ prefixes: { "": "http://example.com/", xsd: "http://www.w3.org/2001/XMLSchema#" } })
    writer.addQuads(quads)
    let result = ""
    writer.end((error: Error | null, text: string) => {
        if (error) {
            throw error
        }
        result = text
    })
    return result
}

export function rootOnlyQuads(dataset: DatasetCore) {
    const root = new ManifestDataset(dataset, DataFactory).root
    const keep: string[] = [Vocabulary.manifestName, Vocabulary.include]
    return [...dataset.match(root as unknown as Term)].filter(q => keep.includes(q.predicate.value))
}
