import * as Blockly from "blockly"

const EXPANDED = "M 3 5 L 13 5 L 8 11 Z"
const COLLAPSED = "M 5 3 L 11 8 L 5 13 Z"

export class CollapseIcon extends Blockly.icons.Icon {
    static readonly type = new Blockly.icons.IconType<CollapseIcon>("collapse")
    static readonly extension = "collapse_icon"

    private static registered = false
    private path: SVGPathElement | null = null

    static register() {
        if (!CollapseIcon.registered) {
            CollapseIcon.registered = true
            Blockly.icons.registry.register(CollapseIcon.type, CollapseIcon)
            Blockly.Extensions.register(CollapseIcon.extension, function (this: Blockly.Block) {
                this.addIcon(new CollapseIcon(this))
            })
        }
    }

    override getType() {
        return CollapseIcon.type
    }

    override initView(pointerdownListener: (e: PointerEvent) => void) {
        if (this.svgRoot) {
            return
        }
        super.initView(pointerdownListener)
        this.path = Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.PATH,
            { d: this.isCollapsed() ? COLLAPSED : EXPANDED, fill: "#fff", class: "blocklyIconSymbol" },
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
        return -1
    }

    override isShownWhenCollapsed() {
        return true
    }

    override updateCollapsed() {
        this.path?.setAttribute("d", this.isCollapsed() ? COLLAPSED : EXPANDED)
    }

    override onClick() {
        this.sourceBlock.setCollapsed(!this.sourceBlock.isCollapsed())
    }

    private isCollapsed() {
        return this.sourceBlock.isCollapsed()
    }
}
