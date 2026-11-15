import * as Blockly from "blockly"
import { Vocabulary, XSD } from "./vocabulary/mod.ts"

interface BlockEntry {
    json: object
    rdf: {
        root?: boolean
        fields?: Record<string, { predicate: string }>
        statements?: Record<string, { predicate: string }>
        literal?: { field: string, datatype: string }
    }
}

export const blockTable: Record<string, BlockEntry> = {
    lws_manifest: {
        json: {
            type: "lws_manifest",
            message0: "manifest %1",
            args0: [{ type: "field_input", name: "MANIFEST_NAME", text: "" }],
            message1: "include %1",
            args1: [{ type: "input_statement", name: "INCLUDES", check: "lws_include" }],
            colour: 210
        },
        rdf: {
            root: true,
            fields: { MANIFEST_NAME: { predicate: Vocabulary.manifestName } },
            statements: { INCLUDES: { predicate: Vocabulary.include } }
        }
    },
    lws_include: {
        json: {
            type: "lws_include",
            message0: "path %1",
            args0: [{ type: "field_input", name: "PATH", text: "" }],
            previousStatement: "lws_include",
            nextStatement: "lws_include",
            colour: 120
        },
        rdf: {
            literal: { field: "PATH", datatype: XSD.anyURI }
        }
    }
}

export const toolbox = {
    kind: "flyoutToolbox",
    contents: [
        { kind: "block", type: "lws_manifest" },
        { kind: "block", type: "lws_include" }
    ]
}

export function defineBlocks() {
    const definitions = Blockly.common.createBlockDefinitionsFromJsonArray(
        Object.values(blockTable).map(entry => entry.json)
    )
    Blockly.common.defineBlocks(definitions)
}
