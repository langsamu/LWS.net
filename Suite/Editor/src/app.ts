import * as Blockly from "blockly"
import { defineBlocks, toolbox } from "./blocks.ts"
import { datasetToState, parse, quadsToTurtle, stateToQuads, type State } from "./rdf.ts"

class ManifestEditor extends HTMLElement {
    workspace!: Blockly.WorkspaceSvg

    connectedCallback() {
        defineBlocks()
        const host = document.createElement("div")
        host.className = "host"
        this.append(host)
        this.workspace = Blockly.inject(host, { toolbox })
        new ResizeObserver(() => Blockly.svgResize(this.workspace)).observe(this)
        this.workspace.addChangeListener((event: Blockly.Events.Abstract) => {
            if (event.isUiEvent) {
                return
            }
            this.dispatchEvent(new CustomEvent("state-changed", { detail: Blockly.serialization.workspaces.save(this.workspace) }))
        })
    }

    load(state: State) {
        Blockly.serialization.workspaces.load(state, this.workspace)
        this.workspace.scrollCenter()
    }
}

class TtlPane extends HTMLElement {
    pre!: HTMLPreElement

    connectedCallback() {
        this.pre = document.createElement("pre")
        this.append(this.pre)
    }

    show(turtle: string) {
        this.pre.textContent = turtle
    }
}

customElements.define("manifest-editor", ManifestEditor)
customElements.define("ttl-pane", TtlPane)

const editor = document.querySelector<ManifestEditor>("manifest-editor")!
const pane = document.querySelector<TtlPane>("ttl-pane")!
const download = document.querySelector<HTMLAnchorElement>("#download")!

editor.addEventListener("state-changed", (event: Event) => {
    const turtle = quadsToTurtle(stateToQuads((event as CustomEvent<State>).detail))
    pane.show(turtle)
    if (download.href) {
        URL.revokeObjectURL(download.href)
    }
    download.href = URL.createObjectURL(new Blob([turtle], { type: "text/turtle" }))
})

const text = await (await fetch("../Model/Resources/new.ttl")).text()
editor.load(datasetToState(parse(text)))
