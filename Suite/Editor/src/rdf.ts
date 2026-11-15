import { DataFactory, Parser, Writer } from "n3"
import type { DatasetCore, Quad, Term } from "@rdfjs/types"
import dataset from "@rdfjs/dataset"
import type { BlockKind, Schema } from "./schema/mod.ts"
import { Vocabulary, XSD } from "./vocabulary/mod.ts"
import { ManifestDataset } from "./wrapping/ManifestDataset.ts"

const { namedNode, blankNode, literal, quad } = DataFactory

export interface Block {
    type: string
    deletable?: boolean
    fields?: Record<string, string | number>
    inputs?: Record<string, { block?: Block }>
    next?: { block?: Block }
}

export interface State {
    blocks?: { blocks?: Block[] }
}

export function parse(text: string) {
    return dataset.dataset(new Parser().parse(text))
}

function rootOf(schema: Schema) {
    return schema.blocks.find(block => block.connection === "root")!
}

function memberFor(block: BlockKind, predicate: string) {
    return block.members.find(member => member.predicate === predicate)!
}

function itemBlockFor(schema: Schema, predicate: string) {
    return schema.blocks.find(block => block.literal && block.predicate === predicate)!
}

export function datasetToState(dataset: DatasetCore, schema: Schema) {
    const manifest = new ManifestDataset(dataset, DataFactory).root
    const root = rootOf(schema)
    const item = itemBlockFor(schema, Vocabulary.include)
    const itemField = item.members[0].name
    let next: Block | undefined
    for (const path of [...manifest.include].reverse()) {
        next = { type: item.type, fields: { [itemField]: path }, ...(next && { next: { block: next } }) }
    }
    const block: Block = {
        type: root.type,
        deletable: false,
        fields: { [memberFor(root, Vocabulary.manifestName).name]: manifest.manifestName }
    }
    if (next) {
        block.inputs = { [memberFor(root, Vocabulary.include).name]: { block: next } }
    }
    return { blocks: { languageVersion: 0, blocks: [block] } }
}

function* chain(first: Block | undefined) {
    for (let block = first; block; block = block.next?.block) {
        yield block
    }
}

function datatypeOf(datatype: string) {
    return datatype === XSD.string ? undefined : namedNode(datatype)
}

function walk(block: Block, quads: Quad[], kinds: Map<string, BlockKind>) {
    const kind = kinds.get(block.type)!
    const subject = blankNode()
    for (const member of kind.members) {
        if (member.kind === "field") {
            const value = block.fields?.[member.name]
            if (value !== undefined && value !== "") {
                quads.push(quad(subject, namedNode(member.predicate), literal(String(value), datatypeOf(member.datatype))))
            }
        } else if (member.kind === "value") {
            const child = block.inputs?.[member.name]?.block
            if (child) {
                quads.push(quad(subject, namedNode(member.predicate), walk(child, quads, kinds)))
            }
        } else {
            for (const child of chain(block.inputs?.[member.name]?.block)) {
                const childKind = kinds.get(child.type)!
                if (childKind.literal) {
                    const field = childKind.members[0]
                    if (field.kind === "field") {
                        quads.push(quad(subject, namedNode(member.predicate), literal(String(child.fields?.[field.name] ?? ""), datatypeOf(field.datatype))))
                    }
                } else {
                    quads.push(quad(subject, namedNode(member.predicate), walk(child, quads, kinds)))
                }
            }
        }
    }
    return subject
}

export function stateToQuads(state: State, schema: Schema) {
    const quads: Quad[] = []
    const kinds = new Map(schema.blocks.map(block => [block.type, block]))
    const root = rootOf(schema)
    for (const block of state.blocks?.blocks ?? []) {
        if (block.type === root.type) {
            walk(block, quads, kinds)
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
