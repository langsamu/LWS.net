import * as Blockly from "blockly"

export const BASE_URI = "baseUri"

export interface Names {
    steps: Set<string>
    assert: Set<string>
    extract: Set<string>
    to: Set<string>
}

export interface Entry {
    block: Blockly.Block
    field: string
    name: string
}

let names: Names = { steps: new Set(), assert: new Set(), extract: new Set(), to: new Set() }

function before(block: Blockly.Block) {
    const previous = block.getPreviousBlock()
    return previous?.getNextBlock() === block ? previous : null
}

function chain(first: Blockly.Block | null) {
    const result: Blockly.Block[] = []
    for (let block = first; block; block = block.getNextBlock()) {
        result.push(block)
    }
    return result
}

function extracted(steps: Blockly.Block[]) {
    return steps.flatMap(step => step.inputList
        .filter(input => names.extract.has(input.name))
        .flatMap(input => chain(input.connection?.targetBlock() ?? null))
        .flatMap(extractor => [...names.to].map(field => ({ block: extractor, field, name: extractor.getFieldValue(field) as string | null })))
        .filter((entry): entry is Entry => entry.name !== null))
}

export function inScope(block: Blockly.Block) {
    let current: Blockly.Block | null = block
    while (current) {
        let head: Blockly.Block = current
        for (let previous = before(head); previous; previous = before(head)) {
            head = previous
        }
        const parent: Blockly.Block | null = head.getParent()
        const input = parent?.getInputWithBlock(head)
        if (parent && input && names.steps.has(input.name)) {
            const earlier: Blockly.Block[] = []
            for (let previous = before(current); previous; previous = before(previous)) {
                earlier.unshift(previous)
            }
            return extracted(earlier)
        }
        if (parent && input && names.assert.has(input.name)) {
            return extracted(parent.inputList
                .filter(candidate => names.steps.has(candidate.name))
                .flatMap(candidate => chain(candidate.connection?.targetBlock() ?? null)))
        }
        current = parent
    }
    return []
}

export function configureScope(value: Names) {
    names = value
}
