import assert from "node:assert/strict"
import { readFileSync } from "node:fs"
import { test } from "node:test"
import * as Blockly from "blockly"
import { isomorphic } from "rdf-isomorphic"
import { defineBlocks } from "../blocks.ts"
import { datasetToState, parse, quadsToTurtle, rootOnlyQuads, stateToQuads } from "../rdf.ts"

const text = readFileSync(new URL("../../../Model/Resources/new.ttl", import.meta.url), "utf8")
const dataset = parse(text)
const expected = rootOnlyQuads(dataset)

test("pure round trip", () => {
    assert.ok(expected.length > 1)
    assert.ok(isomorphic(expected, stateToQuads(datasetToState(dataset))))
})

test("turtle round trip", () => {
    const turtle = quadsToTurtle(stateToQuads(datasetToState(dataset)))
    assert.ok(isomorphic(expected, [...parse(turtle)]))
    assert.ok(turtle.includes("xsd:anyURI"))
})

test("headless workspace round trip", () => {
    defineBlocks()
    const workspace = new Blockly.Workspace()
    Blockly.serialization.workspaces.load(datasetToState(dataset), workspace)
    const saved = Blockly.serialization.workspaces.save(workspace)
    assert.ok(isomorphic(expected, stateToQuads(saved)))
    workspace.dispose()
})
