import * as Blockly from "blockly"
import type { BlockKind } from "./schema/mod.ts"

function words(iri: string) {
    const name = iri.slice(Math.max(iri.lastIndexOf("/"), iri.lastIndexOf("#")) + 1)
    return name.replace(/([a-z0-9])([A-Z])/g, "$1 $2").replaceAll("_", " ")
}

export function searchKeywords(block: BlockKind): string[] {
    const terms = [
        block.class && words(block.class),
        block.predicate && words(block.predicate),
        block.label,
        block.tooltip,
        ...block.members.flatMap(member => [words(member.predicate), member.label, member.tooltip]),
    ]
    return [...new Set(terms.filter((term): term is string => !!term).map(term => term.toLowerCase()))]
}

class KeywordsField extends Blockly.FieldLabel {
    private readonly keywords: string

    constructor(keywords: string) {
        super("")
        this.keywords = keywords
    }

    override getText() {
        return this.keywords
    }

    protected override getDisplayText_() {
        return ""
    }
}

export function searchExtension(block: BlockKind) {
    const name = `search_keywords_${block.type}`
    const keywords = searchKeywords(block).join(" ")
    if (!Blockly.Extensions.isRegistered(name)) {
        Blockly.Extensions.register(name, function (this: Blockly.Block) {
            const input = this.inputList[0] ?? this.appendDummyInput()
            input.insertFieldAt(0, new KeywordsField(keywords))
        })
    }
    return name
}
