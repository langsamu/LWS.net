import * as Blockly from "blockly"
import { Vocabulary } from "../vocabulary/mod.ts"
import { BASE_URI, configureScope, inScope } from "./variableScope.ts"
import type { Schema } from "../schema/mod.ts"

export { BASE_URI }

export class FieldVariableName extends Blockly.FieldDropdown {
    static readonly type = "lws_field_variable_name"

    private static registered = false

    static register(schema: Schema) {
        const collect = (predicate: string) => new Set(schema.blocks.flatMap(block => block.members.filter(member => member.predicate === predicate).map(member => member.name)))
        configureScope({ steps: collect(Vocabulary.steps), assert: collect(Vocabulary.assert), extract: collect(Vocabulary.extract), to: collect(Vocabulary.to) })
        if (!FieldVariableName.registered) {
            FieldVariableName.registered = true
            Blockly.fieldRegistry.register(FieldVariableName.type, FieldVariableName)
        }
    }

    constructor(value?: string) {
        super(() => this.computeOptions())
        this.setValue(value ?? "")
    }

    static override fromJson(options: Blockly.FieldDropdownFromJsonConfig & { text?: string }) {
        return new FieldVariableName(options.text ?? "")
    }

    private computeOptions(): [string, string][] {
        const block = this.getSourceBlock()
        const available = block && !block.isInFlyout ? inScope(block).map(entry => entry.name) : []
        const current = this.getValue() as string | null
        const values = [...new Set([BASE_URI, ...available, current].filter((value): value is string => !!value?.trim()))]
        return values.map(value => [value, value])
    }

    override getOptions(): [string, string][] {
        return this.computeOptions()
    }

    protected override doClassValidation_(newValue?: string): string | null {
        return newValue === undefined || newValue === null ? null : String(newValue)
    }
}
