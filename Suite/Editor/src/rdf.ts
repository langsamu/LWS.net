import { DataFactory, Parser } from "n3"
import Serializer from "@rdfjs/serializer-turtle"
import PrefixMap from "@rdfjs/prefix-map/PrefixMap.js"
import type { DatasetCore, Quad, Term } from "@rdfjs/types"
import dataset from "@rdfjs/dataset"
import { choose, type BlockKind, type NodeFacts, type Schema, type StatementInput } from "./schema/mod.ts"
import { RDF, Vocabulary, XSD } from "./vocabulary/mod.ts"
import { ManifestDataset } from "./wrapping/ManifestDataset.ts"

const { namedNode, blankNode, literal, quad } = DataFactory

export interface Block {
    type: string
    deletable?: boolean
    collapsed?: boolean
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

function objects(dataset: DatasetCore, subject: Term, predicate: string) {
    return [...dataset.match(subject, namedNode(predicate))].map(q => q.object)
}

function listItems(dataset: DatasetCore, head: Term) {
    const items: Term[] = []
    const seen = new Set<string>()
    let node = head
    while (!(node.termType === "NamedNode" && node.value === RDF.nil)) {
        const key = node.termType + node.value
        const [first] = objects(dataset, node, RDF.first)
        const [rest] = objects(dataset, node, RDF.rest)
        if (seen.has(key) || !first || !rest) {
            break
        }
        seen.add(key)
        items.push(first)
        node = rest
    }
    return items
}

function factsOf(dataset: DatasetCore, node: Term): NodeFacts {
    const predicates = new Set<string>()
    const types = new Set<string>()
    for (const q of dataset.match(node)) {
        predicates.add(q.predicate.value)
        if (q.predicate.value === RDF.type) {
            types.add(q.object.value)
        }
    }
    return { predicates, types }
}

function datatypeMatches(dataset: DatasetCore, node: Term, kind: BlockKind) {
    return kind.members.every(member => {
        if (member.kind !== "field") {
            return true
        }
        return objects(dataset, node, member.predicate).every(object => object.termType !== "Literal" || object.datatype.value === member.datatype)
    })
}

class Loader {
    private readonly dataset: DatasetCore
    private readonly schema: Schema

    constructor(dataset: DatasetCore, schema: Schema) {
        this.dataset = dataset
        this.schema = schema
    }

    private resolve(target: string, node: Term) {
        const abstract = this.schema.abstracts.find(candidate => candidate.class === target)
        const first = abstract ? choose(this.schema, abstract, factsOf(this.dataset, node)) : this.schema.blocks.find(kind => kind.class === target)
        if (!first) {
            return undefined
        }
        return this.schema.blocks.filter(kind => kind.class === first.class).find(kind => datatypeMatches(this.dataset, node, kind)) ?? first
    }

    load(kind: BlockKind, node: Term): Block {
        const block: Block = { type: kind.type }
        const fields: Record<string, string | number> = {}
        const inputs: Record<string, { block?: Block }> = {}
        for (const member of kind.members) {
            const values = objects(this.dataset, node, member.predicate)
            if (member.kind === "field") {
                const value = values.find(object => object.termType === "Literal")
                if (value) {
                    fields[member.name] = member.datatype === XSD.integer ? Number(value.value) : value.value
                }
            } else if (member.kind === "value") {
                const blocks = this.blocks(member.target, values)
                if (blocks.length) {
                    inputs[member.name] = { block: blocks[0] }
                }
            } else {
                const items = member.list ? values.flatMap(value => listItems(this.dataset, value)) : values
                const head = this.chain(member.target === undefined ? this.literals(member, items) : this.blocks(member.target, items))
                if (head) {
                    inputs[member.name] = { block: head }
                }
            }
        }
        if (Object.keys(fields).length) {
            block.fields = fields
        }
        if (Object.keys(inputs).length) {
            block.inputs = inputs
        }
        return block
    }

    private blocks(target: string, nodes: Term[]) {
        const blocks: Block[] = []
        for (const node of nodes) {
            const kind = this.resolve(target, node)
            if (kind) {
                blocks.push(this.load(kind, node))
            }
        }
        return blocks
    }

    private literals(member: StatementInput, values: Term[]) {
        const blocks: Block[] = []
        for (const value of values) {
            const item = value.termType === "Literal" && this.schema.blocks.find(kind => kind.literal && kind.predicate === member.predicate && kind.members[0].kind === "field" && kind.members[0].datatype === value.datatype.value)
            if (item) {
                blocks.push({ type: item.type, fields: { [item.members[0].name]: value.value } })
            }
        }
        return blocks
    }

    private chain(blocks: Block[]) {
        let next: Block | undefined
        for (const block of [...blocks].reverse()) {
            next = { ...block, ...(next && { next: { block: next } }) }
        }
        return next
    }
}

export function datasetToState(dataset: DatasetCore, schema: Schema) {
    const manifest = new ManifestDataset(dataset, DataFactory).root as unknown as Term
    const block: Block = { ...new Loader(dataset, schema).load(rootOf(schema), manifest), deletable: false }
    const kinds = new Map(schema.blocks.map(kind => [kind.type, kind]))
    for (const input of Object.values(block.inputs ?? {})) {
        for (const child of chain(input.block)) {
            collapse(child, kinds)
        }
    }
    return { blocks: { languageVersion: 0, blocks: [block] } }
}

export function emptyState(schema: Schema) {
    return { blocks: { languageVersion: 0, blocks: [{ type: rootOf(schema).type, deletable: false }] } }
}

function collapse(block: Block, kinds: Map<string, BlockKind>) {
    if (kinds.get(block.type)?.members.some(member => member.kind === "statements")) {
        block.collapsed = true
    }
    for (const input of Object.values(block.inputs ?? {})) {
        for (const child of chain(input.block)) {
            collapse(child, kinds)
        }
    }
}

function* chain(first: Block | undefined) {
    for (let block = first; block; block = block.next?.block) {
        yield block
    }
}

function datatypeOf(datatype: string) {
    return datatype === XSD.string ? undefined : namedNode(datatype)
}

function objectOf(child: Block, quads: Quad[], kinds: Map<string, BlockKind>): Quad["object"] {
    const kind = kinds.get(child.type)!
    if (kind.literal) {
        const field = kind.members[0]
        return literal(field.kind === "field" ? String(child.fields?.[field.name] ?? "") : "", field.kind === "field" ? datatypeOf(field.datatype) : undefined)
    }
    return walk(child, quads, kinds)
}

let nodeCount = 0

function node() {
    return blankNode(`b${nodeCount++}`)
}

function listOf(children: Block[], quads: Quad[], kinds: Map<string, BlockKind>): Quad["object"] {
    const nodes = children.map(() => node())
    children.forEach((child, index) => {
        quads.push(quad(nodes[index], namedNode(RDF.first), objectOf(child, quads, kinds)))
        quads.push(quad(nodes[index], namedNode(RDF.rest), nodes[index + 1] ?? namedNode(RDF.nil)))
    })
    return nodes[0] ?? namedNode(RDF.nil)
}

function walk(block: Block, quads: Quad[], kinds: Map<string, BlockKind>) {
    const kind = kinds.get(block.type)!
    const subject = node()
    for (const type of kind.types) {
        quads.push(quad(subject, namedNode(RDF.type), namedNode(type)))
    }
    for (const member of kind.members) {
        if (member.kind === "field") {
            const value = block.fields?.[member.name]
            if (value !== undefined && (value !== "" || member.required)) {
                quads.push(quad(subject, namedNode(member.predicate), literal(String(value), datatypeOf(member.datatype))))
            }
        } else if (member.kind === "value") {
            const child = block.inputs?.[member.name]?.block
            if (child) {
                quads.push(quad(subject, namedNode(member.predicate), walk(child, quads, kinds)))
            }
        } else {
            const children = [...chain(block.inputs?.[member.name]?.block)]
            if (member.list) {
                quads.push(quad(subject, namedNode(member.predicate), listOf(children, quads, kinds)))
            } else {
                for (const child of children) {
                    quads.push(quad(subject, namedNode(member.predicate), objectOf(child, quads, kinds)))
                }
            }
        }
    }
    return subject
}

export function stateToQuads(state: State, schema: Schema) {
    const quads: Quad[] = []
    nodeCount = 0
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
    const prefixes = new PrefixMap([["", DataFactory.namedNode("http://example.com/")], ["xsd", DataFactory.namedNode("http://www.w3.org/2001/XMLSchema#")]], { factory: DataFactory })
    return new Serializer({ prefixes }).transform(quads)
}
