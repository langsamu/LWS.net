import * as Blockly from "blockly"
import { defineBlocks, toolbox } from "../blocks.ts"
import type { State } from "../rdf.ts"

export class ManifestEditor extends HTMLElement {
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
