import assert from "node:assert/strict"
import { readFileSync } from "node:fs"
import { test } from "node:test"
import * as Blockly from "blockly"
import { isomorphic } from "rdf-isomorphic"
import dataset_ from "@rdfjs/dataset"
import { defineBlocks } from "../blocks.ts"
import { buildSchema } from "../schema/mod.ts"
import { datasetToState, parse, quadsToTurtle, rootOnlyQuads, stateToQuads } from "../rdf.ts"

const text = readFileSync(new URL("../../../Model/Resources/new.ttl", import.meta.url), "utf8")
const dataset = parse(text)
const read = (path: string) => readFileSync(new URL(path, import.meta.url), "utf8")
const schema = buildSchema(dataset_.dataset([...parse(read("../../../Model/Resources/vocabulary.ttl")), ...parse(read("../../shapes.ttl"))]))
const expected = rootOnlyQuads(dataset)

test("pure round trip", () => {
    assert.ok(expected.length > 1)
    assert.ok(isomorphic(expected, stateToQuads(datasetToState(dataset, schema), schema)))
})

test("turtle round trip", () => {
    const turtle = quadsToTurtle(stateToQuads(datasetToState(dataset, schema), schema))
    assert.ok(isomorphic(expected, [...parse(turtle)]))
    assert.ok(turtle.includes("xsd:anyURI"))
})

test("headless workspace round trip", () => {
    defineBlocks(schema)
    const workspace = new Blockly.Workspace()
    Blockly.serialization.workspaces.load(datasetToState(dataset, schema), workspace)
    const saved = Blockly.serialization.workspaces.save(workspace)
    assert.ok(isomorphic(expected, stateToQuads(saved, schema)))
    workspace.dispose()
})
