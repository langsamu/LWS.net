import * as Blockly from "blockly"
import { Vocabulary, XSD } from "./vocabulary/mod.ts"
import { CollapseIcon } from "./components/CollapseIcon.ts"
import { OpenIncludeIcon } from "./components/OpenIncludeIcon.ts"
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
    const text = (...parts: string[]) => parts.filter(Boolean).join(" ")
    const lines: { message: string, args: Json[] }[] = [{ message: block.label, args: [] }]
    block.members.forEach((member, index) => {
        const args = memberArgs(member)
        if (index === 0 && member.kind === "field") {
            lines[0] = { message: text(block.label, block.literal ? "" : member.label, "%1"), args: [args] }
        } else {
            lines.push({ message: text(member.label, "%1"), args: [args] })
        }
    })
    if (!lines[0].message) {
        lines.shift()
    }
    lines.forEach((line, index) => {
        definition[`message${index}`] = line.message
        if (line.args.length) {
            definition[`args${index}`] = line.args
        }
    })
    definition.inputsInline = !block.members.some(member => member.kind === "statements") && block.members.filter(member => member.kind === "value").length !== 1
    switch (block.connection) {
        case "value":
            definition.output = block.checks
            break
        case "statement":
            definition.previousStatement = block.checks
            definition.nextStatement = block.checks
            break
    }
    if (block.members.some(member => member.kind === "statements")) {
        definition.extensions = [CollapseIcon.extension]
    }
    if (block.literal && block.predicate === Vocabulary.include) {
        definition.extensions = [OpenIncludeIcon.extension]
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
    CollapseIcon.register()
    OpenIncludeIcon.register()
    Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(blockDefinitions(schema)))
}
