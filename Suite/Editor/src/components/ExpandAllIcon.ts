import * as Blockly from "blockly"

const EXPAND = "M 3 2 L 13 2 L 8 7 Z M 3 9 L 13 9 L 8 14 Z"
const COLLAPSE = "M 2 3 L 7 8 L 2 13 Z M 9 3 L 14 8 L 9 13 Z"

export class ExpandAllIcon extends Blockly.icons.Icon {
    static readonly type = new Blockly.icons.IconType<ExpandAllIcon>("expand_all")
    static readonly extension = "expand_all_icon"

    private static registered = false
    private path: SVGPathElement | null = null

    static register() {
        if (!ExpandAllIcon.registered) {
            ExpandAllIcon.registered = true
            Blockly.icons.registry.register(ExpandAllIcon.type, ExpandAllIcon)
            Blockly.Extensions.register(ExpandAllIcon.extension, function (this: Blockly.Block) {
                this.addIcon(new ExpandAllIcon(this))
            })
        }
    }

    override getType() {
        return ExpandAllIcon.type
    }

    override initView(pointerdownListener: (e: PointerEvent) => void) {
        if (this.svgRoot) {
            return
        }
        super.initView(pointerdownListener)
        this.path = Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.PATH,
            { d: this.glyph(), fill: "#fff", class: "blocklyIconSymbol" },
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
        return 0
    }

    override isShownWhenCollapsed() {
        return true
    }

    override updateCollapsed() {
        this.path?.setAttribute("d", this.glyph())
    }

    override onClick() {
        const blocks = this.subtree()
        const collapse = !blocks.some(block => block.isCollapsed())
        Blockly.Events.setGroup(true)
        try {
            for (const block of collapse ? [...blocks].reverse() : blocks) {
                block.setCollapsed(collapse)
            }
        } finally {
            Blockly.Events.setGroup(false)
        }
        this.updateCollapsed()
    }

    private subtree() {
        const block = this.sourceBlock
        return [block, ...block.inputList.flatMap(input => input.connection?.targetBlock()?.getDescendants(false) ?? [])]
            .filter(candidate => candidate.hasIcon(ExpandAllIcon.type))
    }

    private glyph() {
        return this.subtree().some(block => block.isCollapsed()) ? EXPAND : COLLAPSE
    }
}
