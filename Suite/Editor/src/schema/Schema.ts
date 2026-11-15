export interface Schema {
    root: string
    blocks: BlockKind[]
    abstracts: AbstractKind[]
    categories: Category[]
}

export interface BlockKind {
    type: string
    class?: string
    predicate?: string
    label: string
    tooltip?: string
    hue?: number
    connection: "root" | "value" | "statement"
    checks: string[]
    required: string[]
    types: string[]
    literal: boolean
    members: Member[]
}

export type Member = Field | ValueInput | StatementInput

export interface Field {
    kind: "field"
    name: string
    label: string
    tooltip?: string
    predicate: string
    datatype: string
    required: boolean
}

export interface ValueInput {
    kind: "value"
    name: string
    label: string
    tooltip?: string
    predicate: string
    target: string
    check: string
    required: boolean
}

export interface StatementInput {
    kind: "statements"
    name: string
    label: string
    tooltip?: string
    predicate: string
    target?: string
    check: string
    list: boolean
    required: boolean
}

export interface AbstractKind {
    class: string
    label: string
    tooltip?: string
    hue?: number
    alternatives: string[]
}

export interface Category {
    name: string
    class?: string
    hue?: number
    blocks: string[]
}
