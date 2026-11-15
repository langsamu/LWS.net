import * as Blockly from "blockly"
import { defineBlocks, toolbox } from "../blocks.ts"
import type { Schema } from "../schema/mod.ts"
import type { State } from "../rdf.ts"

export class ManifestEditor extends HTMLElement {
    workspace!: Blockly.WorkspaceSvg

    init(schema: Schema) {
        defineBlocks(schema)
        const host = document.createElement("div")
        host.className = "host"
        this.append(host)
        this.workspace = Blockly.inject(host, { toolbox: toolbox(schema) })
        new ResizeObserver(() => Blockly.svgResize(this.workspace)).observe(this)
        let pending = false
        this.workspace.addChangeListener((event: Blockly.Events.Abstract) => {
            if (event.isUiEvent || pending) {
                return
            }
            pending = true
            setTimeout(() => {
                pending = false
                this.dispatchEvent(new CustomEvent("state-changed", { detail: Blockly.serialization.workspaces.save(this.workspace) }))
            })
        })
    }

    load(state: State) {
        Blockly.serialization.workspaces.load(state, this.workspace)
        this.workspace.scrollCenter()
    }
}
