import * as Blockly from "blockly"

const ARROW = "M 6 3 L 13 3 L 13 10 M 13 3 L 5 11 M 3 7 L 3 13 L 9 13"

export class OpenIncludeIcon extends Blockly.icons.Icon {
    static readonly type = new Blockly.icons.IconType<OpenIncludeIcon>("open_include")
    static readonly extension = "open_include_icon"
    static readonly event = "open-include"

    private static registered = false

    static register() {
        if (!OpenIncludeIcon.registered) {
            OpenIncludeIcon.registered = true
            Blockly.icons.registry.register(OpenIncludeIcon.type, OpenIncludeIcon)
            Blockly.Extensions.register(OpenIncludeIcon.extension, function (this: Blockly.Block) {
                this.addIcon(new OpenIncludeIcon(this))
            })
            Blockly.ContextMenuRegistry.registry.register({
                id: "open_included_manifest",
                scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
                displayText: "Open included manifest",
                weight: 100,
                preconditionFn: scope => scope.block?.getIcon(OpenIncludeIcon.type) ? "enabled" : "hidden",
                callback: scope => (scope.block?.getIcon(OpenIncludeIcon.type) as OpenIncludeIcon | undefined)?.open()
            })
        }
    }

    override getType() {
        return OpenIncludeIcon.type
    }

    override initView(pointerdownListener: (e: PointerEvent) => void) {
        if (this.svgRoot) {
            return
        }
        super.initView(pointerdownListener)
        Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.PATH,
            { d: ARROW, fill: "none", stroke: "#fff", "stroke-width": 1.5, class: "blocklyIconSymbol" },
            this.svgRoot!
        )
        Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.RECT,
            { width: 16, height: 16, fill: "transparent", style: "cursor: pointer" },
            this.svgRoot!
        )
    }

    override getSize() {
        return new Blockly.utils.Size(16, 16)
    }

    override getWeight() {
        return -2
    }

    override isShownWhenCollapsed() {
        return true
    }

    override onClick() {
        this.open()
    }

    open() {
        const path = this.sourceBlock.inputList.flatMap(input => input.fieldRow).find(field => field.name)?.getValue()
        const workspace = this.sourceBlock.workspace as Blockly.WorkspaceSvg
        if (path) {
            workspace.getInjectionDiv().dispatchEvent(new CustomEvent(OpenIncludeIcon.event, { detail: String(path), bubbles: true }))
        }
    }
}
