import * as Blockly from "blockly"
import { XSD } from "./vocabulary/mod.ts"
import type { BlockKind, Member, Schema } from "./schema/mod.ts"

type Json = Record<string, unknown>

function memberArgs(member: Member): Json {
    switch (member.kind) {
        case "field":
            return member.datatype === XSD.integer
                ? { type: "field_number", name: member.name, value: 0, precision: 1 }
                : { type: "field_input", name: member.name, text: "" }
        case "value":
            return { type: "input_value", name: member.name, check: member.check }
        case "statements":
            return { type: "input_statement", name: member.name, check: member.check }
    }
}

export function blockDefinition(block: BlockKind): Json {
    const definition: Json = { type: block.type, tooltip: block.tooltip ?? "" }
    const lines: { message: string, args: Json[] }[] = [{ message: block.label, args: [] }]
    block.members.forEach((member, index) => {
        const args = memberArgs(member)
        if (index === 0 && member.kind === "field") {
            const prefix = block.literal ? "" : ` ${member.label}`
            lines[0] = { message: `${block.label}${prefix} %1`, args: [args] }
        } else {
            lines.push({ message: `${member.label} %1`, args: [args] })
        }
    })
    lines.forEach((line, index) => {
        definition[`message${index}`] = line.message
        if (line.args.length) {
            definition[`args${index}`] = line.args
        }
    })
    if (block.members.some(member => member.kind === "statements")) {
        definition.inputsInline = false
    }
    switch (block.connection) {
        case "value":
            definition.output = block.checks
            break
        case "statement":
            definition.previousStatement = block.checks
            definition.nextStatement = block.checks
            break
    }
    if (block.hue !== undefined) {
        definition.colour = block.hue
    }
    return definition
}

export function blockDefinitions(schema: Schema) {
    return schema.blocks.map(blockDefinition)
}

export function toolbox(schema: Schema) {
    const types = new Map(schema.blocks.map(block => [block.type, block]))
    return {
        kind: "categoryToolbox",
        contents: schema.categories
            .map(category => ({
                kind: "category",
                name: category.name,
                ...(category.hue !== undefined && { colour: String(category.hue) }),
                contents: category.blocks
                    .filter(type => types.get(type)!.connection !== "root")
                    .map(type => ({ kind: "block", type }))
            }))
            .filter(category => category.contents.length)
    }
}

export function defineBlocks(schema: Schema) {
    Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(blockDefinitions(schema)))
}
