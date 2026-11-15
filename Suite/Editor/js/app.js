import * as Blockly from "blockly"
import { defineBlocks, toolbox } from "./blocks.js"
import { datasetToState, parse, quadsToTurtle, stateToQuads } from "./rdf.js"

class ManifestEditor extends HTMLElement {
    connectedCallback() {
        defineBlocks()
        const host = document.createElement("div")
        host.className = "host"
        this.append(host)
        this.workspace = Blockly.inject(host, { toolbox })
        new ResizeObserver(() => Blockly.svgResize(this.workspace)).observe(this)
        this.workspace.addChangeListener(event => {
            if (event.isUiEvent) {
                return
            }
            this.dispatchEvent(new CustomEvent("state-changed", { detail: Blockly.serialization.workspaces.save(this.workspace) }))
        })
    }

    load(state) {
        Blockly.serialization.workspaces.load(state, this.workspace)
        this.workspace.scrollCenter()
    }
}

class TtlPane extends HTMLElement {
    connectedCallback() {
        this.pre = document.createElement("pre")
        this.append(this.pre)
    }

    show(turtle) {
        this.pre.textContent = turtle
    }
}

customElements.define("manifest-editor", ManifestEditor)
customElements.define("ttl-pane", TtlPane)

const editor = document.querySelector("manifest-editor")
const pane = document.querySelector("ttl-pane")
const download = document.querySelector("#download")

editor.addEventListener("state-changed", event => {
    const turtle = quadsToTurtle(stateToQuads(event.detail))
    pane.show(turtle)
    if (download.href) {
        URL.revokeObjectURL(download.href)
    }
    download.href = URL.createObjectURL(new Blob([turtle], { type: "text/turtle" }))
})

const text = await (await fetch("../Model/Resources/new.ttl")).text()
editor.load(datasetToState(parse(text)))
