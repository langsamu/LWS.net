import type { AbstractKind, BlockKind, Schema } from "./Schema.ts"

export interface NodeFacts {
    predicates: ReadonlySet<string>
    types: ReadonlySet<string>
}

export function matches(kind: Pick<BlockKind, "required" | "types">, node: NodeFacts) {
    return kind.required.every(predicate => node.predicates.has(predicate)) && kind.types.every(type => node.types.has(type))
}

export function choose(schema: Schema, abstract: string | AbstractKind, node: NodeFacts): BlockKind | undefined {
    const kind = typeof abstract === "string" ? schema.abstracts.find(candidate => candidate.class === abstract) : abstract
    for (const alternative of kind?.alternatives ?? []) {
        const nested = schema.abstracts.find(candidate => candidate.class === alternative)
        const block = nested ? choose(schema, nested, node) : schema.blocks.find(candidate => candidate.class === alternative && matches(candidate, node))
        if (block) {
            return block
        }
    }
    return undefined
}
