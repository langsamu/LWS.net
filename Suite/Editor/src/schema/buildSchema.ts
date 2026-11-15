import { DataFactory } from "n3"
import type { DatasetCore } from "@rdfjs/types"
import { RDF } from "../vocabulary/mod.ts"
import type { NodeShape } from "../wrapping/NodeShape.ts"
import type { PropertyShape } from "../wrapping/PropertyShape.ts"
import { ShapesDataset } from "../wrapping/ShapesDataset.ts"
import type { AbstractKind, BlockKind, Category, Member, Schema } from "./Schema.ts"

type Connection = "root" | "value" | "statement"

interface Common {
    name: string
    label: string
    tooltip?: string
    predicate: string
    required: boolean
}

function localName(iri: string) {
    return iri.slice(Math.max(iri.lastIndexOf("/"), iri.lastIndexOf("#")) + 1)
}

function fieldName(iri: string) {
    return localName(iri).replace(/([a-z0-9])([A-Z])/g, "$1_$2").toUpperCase()
}

function byOrder(a: PropertyShape, b: PropertyShape) {
    return (a.order ?? Infinity) - (b.order ?? Infinity)
}

export function buildSchema(dataset: DatasetCore): Schema {
    return new SchemaBuilder(new ShapesDataset(dataset, DataFactory)).build()
}

class SchemaBuilder {
    private readonly blocks: BlockKind[] = []
    private readonly abstracts = new Map<string, AbstractKind>()
    private readonly shapesByIri = new Map<string, NodeShape>()
    private readonly usage = new Map<string, Set<Connection>>()
    private readonly keys = new Map<BlockKind, string>()
    private readonly queue: NodeShape[] = []
    private readonly seen = new Set<string>()

    private readonly shapes: ShapesDataset

    constructor(shapes: ShapesDataset) {
        this.shapes = shapes
        for (const shape of shapes.shapes) {
            this.shapesByIri.set(shape.value, shape)
        }
    }

    build(): Schema {
        const root = this.shapes.root
        this.enqueue(root, "root")
        for (let next = this.queue.shift(); next; next = this.queue.shift()) {
            this.expand(next)
        }
        for (const [block, key] of this.keys) {
            const connections = this.usage.get(key)!
            if (connections.size !== 1) {
                throw new Error(`Block ${block.type} is used as ${[...connections].join(" and ")}`)
            }
            block.connection = [...connections][0]
        }
        return { root: root.value, blocks: this.blocks, abstracts: [...this.abstracts.values()], categories: this.categories() }
    }

    private categories() {
        const categories: Category[] = []
        const categorised = new Set<BlockKind>()
        for (const abstract of this.abstracts.values()) {
            const blocks: BlockKind[] = []
            for (const concrete of this.concrete(this.shapesByIri.get(abstract.class)!)) {
                blocks.push(...this.blocks.filter(block => block.class === concrete.value))
            }
            blocks.forEach(block => categorised.add(block))
            categories.push({ name: abstract.label, class: abstract.class, hue: abstract.hue, blocks: blocks.map(block => block.type) })
        }
        const rest = this.blocks.filter(block => !categorised.has(block))
        return [{ name: "Structure", blocks: rest.map(block => block.type) }, ...categories]
    }

    private lineage(shape: NodeShape) {
        const lineage = [shape]
        for (const current of lineage) {
            for (const parent of current.superClasses) {
                if (!lineage.some(known => known.value === parent.value)) {
                    lineage.push(parent)
                }
            }
        }
        return lineage
    }

    private hue(lineage: NodeShape[]) {
        return lineage.find(shape => shape.hue !== undefined)?.hue
    }

    private concrete(shape: NodeShape): NodeShape[] {
        if (!shape.isAbstract) {
            return [shape]
        }
        const alternatives = shape.alternatives
        if (!alternatives) {
            throw new Error(`Abstract shape ${shape.value} has no alternatives`)
        }
        this.abstracts.set(shape.value, {
            class: shape.value,
            label: shape.name ?? shape.label ?? localName(shape.value),
            tooltip: shape.comment,
            hue: this.hue(this.lineage(shape)),
            alternatives: [...alternatives].map(alternative => alternative.value),
        })
        return [...alternatives].flatMap(alternative => this.concrete(alternative))
    }

    private enqueue(shape: NodeShape, connection: Connection) {
        const connections = this.usage.get(shape.value) ?? new Set()
        connections.add(connection)
        this.usage.set(shape.value, connections)
        if (!this.seen.has(shape.value)) {
            this.seen.add(shape.value)
            this.queue.push(shape)
        }
    }

    private inherited(lineage: NodeShape[]) {
        const merged = new Map<string, PropertyShape>()
        for (const shape of [...lineage].reverse()) {
            for (const property of shape.properties) {
                merged.set(property.path.value, property)
            }
        }
        return [...merged.values()]
    }

    private expand(shape: NodeShape) {
        const lineage = this.lineage(shape)
        const inherited = this.inherited(lineage)
        const properties = inherited
            .filter(property => property.described && property.hasValue === undefined && property.maxCount !== 0)
            .sort(byOrder)
        const required = properties.filter(property => (property.minCount ?? 0) >= 1).map(property => property.described!.value)
        const types = inherited
            .filter(property => property.described?.value === RDF.type && property.hasValue !== undefined)
            .map(property => property.hasValue!)
        const label = shape.name ?? shape.label ?? localName(shape.value)

        let combinations: Map<string, string>[] = [new Map()]
        for (const property of properties.filter(property => !property.node && property.maxCount === 1 && property.datatypes.length > 1)) {
            combinations = combinations.flatMap(combination => property.datatypes.map(datatype => new Map(combination).set(property.described!.value, datatype)))
        }

        for (const combination of combinations) {
            const members: Member[] = []
            for (const property of properties) {
                members.push(this.member(shape, property, combination))
            }
            const datatypes = [...combination.values()].map(localName)
            const block: BlockKind = {
                type: ["lws", localName(shape.value).toLowerCase(), ...datatypes].filter(Boolean).join("_"),
                class: shape.value,
                label: datatypes.length && label ? `${label} (${datatypes.join(", ")})` : label,
                tooltip: shape.comment,
                hue: this.hue(lineage),
                connection: "value",
                checks: lineage.map(parent => parent.value),
                required,
                types,
                literal: false,
                members,
            }
            this.blocks.push(block)
            this.keys.set(block, shape.value)
        }
    }

    private member(shape: NodeShape, property: PropertyShape, combination: Map<string, string>): Member {
        const predicate = property.described!
        const common: Common = {
            name: fieldName(predicate.value),
            label: property.name ?? predicate.label ?? localName(predicate.value),
            tooltip: predicate.comment,
            predicate: predicate.value,
            required: (property.minCount ?? 0) >= 1,
        }
        const single = property.maxCount === 1
        const node = property.node
        if (node) {
            const item = node.listItem
            if (item) {
                for (const concrete of this.concrete(item)) {
                    this.enqueue(concrete, "statement")
                }
                return { kind: "statements", ...common, target: item.value, check: item.value, list: true }
            }
            for (const concrete of this.concrete(node)) {
                this.enqueue(concrete, single ? "value" : "statement")
            }
            return single
                ? { kind: "value", ...common, target: node.value, check: node.value }
                : { kind: "statements", ...common, target: node.value, check: node.value, list: false }
        }
        if (single) {
            return { kind: "field", ...common, datatype: combination.get(common.predicate) ?? property.datatypes[0], options: property.in }
        }
        this.literalItems(shape, property, common, predicate.label ?? localName(predicate.value))
        return { kind: "statements", ...common, check: common.predicate, list: false }
    }

    private literalItems(shape: NodeShape, property: PropertyShape, common: Common, label: string) {
        const key = "literal:" + common.predicate
        if (this.seen.has(key)) {
            return
        }
        this.seen.add(key)
        for (const datatype of property.datatypes) {
            this.blocks.push({
                type: ["lws", localName(common.predicate).toLowerCase(), property.datatypes.length > 1 && localName(datatype)].filter(Boolean).join("_"),
                predicate: common.predicate,
                label,
                tooltip: common.tooltip,
                hue: this.hue(this.lineage(shape)),
                connection: "statement",
                checks: [common.predicate],
                required: [],
                types: [],
                literal: true,
                members: [{ kind: "field", ...common, required: true, datatype }],
            })
        }
    }
}
