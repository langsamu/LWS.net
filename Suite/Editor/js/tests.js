import * as Blockly from "blockly"
import { defineBlocks } from "./blocks.js"
import { datasetToState, isomorphic, parse, quadsToTurtle, rootOnlyQuads, stateToQuads } from "./rdf.js"

const results = document.querySelector("#results")
let failed = 0

function assert(condition, name) {
    if (!condition) {
        failed++
    }
    const item = document.createElement("li")
    item.className = condition ? "pass" : "fail"
    item.textContent = `${condition ? "PASS" : "FAIL"} ${name}`
    results.append(item)
}

function test(name, body) {
    try {
        body()
    } catch (error) {
        assert(false, `${name}: ${error.message}`)
    }
}

try {
    const text = await (await fetch("../Model/Resources/new.ttl")).text()
    const dataset = parse(text)
    const expected = rootOnlyQuads(dataset)

    test("pure round trip", () => {
        assert(expected.length > 1, "expected quads present")
        assert(isomorphic(expected, stateToQuads(datasetToState(dataset))), "dataset -> state -> quads is isomorphic")
    })

    test("turtle round trip", () => {
        const turtle = quadsToTurtle(stateToQuads(datasetToState(dataset)))
        assert(isomorphic(expected, [...parse(turtle)]), "dataset -> state -> turtle -> dataset is isomorphic")
        assert(turtle.includes("xsd:anyURI"), "turtle uses xsd prefix")
    })

    test("headless workspace round trip", () => {
        defineBlocks()
        const workspace = new Blockly.Workspace()
        Blockly.serialization.workspaces.load(datasetToState(dataset), workspace)
        const saved = Blockly.serialization.workspaces.save(workspace)
        assert(isomorphic(expected, stateToQuads(saved)), "state -> headless workspace -> state -> quads is isomorphic")
        workspace.dispose()
    })
} catch (error) {
    assert(false, `setup: ${error.message}`)
}

document.title = failed === 0 ? "PASS" : "FAIL"
