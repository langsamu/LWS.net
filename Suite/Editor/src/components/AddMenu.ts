import * as Blockly from "blockly"
import type { BlockKind, Member, Schema } from "../schema/mod.ts"

export interface AddEntry {
    id: string
    owner: string
    member: string
    memberKind: "value" | "statements"
    type: string
    text: string
}

const words = (text: string) => text.replace(/([a-z0-9])([A-Z])/g, "$1 $2").replace(/[_\s]+/g, " ").trim().toLowerCase()
const local = (iri: string) => iri.slice(Math.max(iri.lastIndexOf("#"), iri.lastIndexOf("/")) + 1)

export function addEntries(schema: Schema, owner: BlockKind): AddEntry[] {
    const connectable = (member: Member) => member.kind === "value" ? "value" : "statement"
    const entries: AddEntry[] = []
    for (const member of owner.members) {
        if (member.kind === "field") {
            continue
        }
        const abstract = schema.abstracts.find(candidate => candidate.class === member.check)
        const prefix = abstract ? `${words(abstract.label || local(abstract.class))}: ` : ""
        const candidates = schema.blocks.filter(block => block.connection === connectable(member) && block.checks.includes(member.check))
        const labels = candidates.map(block => block.label || words(block.class ? local(block.class) : block.type))
        candidates.forEach((block, index) => {
            const duplicated = labels.filter(label => label === labels[index]).length > 1
            const item = prefix + labels[index] + (duplicated ? ` (${block.type.slice(block.type.lastIndexOf("_") + 1)})` : "")
            entries.push({
                id: `add_${owner.type}_${member.name}_${block.type}`,
                owner: owner.type,
                member: member.name,
                memberKind: member.kind,
                type: block.type,
                text: member.kind === "value" ? `Set ${member.label || words(member.name)}: ${item}` : `Add ${item}`
            })
        })
    }
    return entries
}

function create(workspace: Blockly.WorkspaceSvg, type: string) {
    const block = workspace.newBlock(type)
    block.initSvg()
    block.render()
    return block
}

function apply(entry: AddEntry, parent: Blockly.BlockSvg) {
    const input = parent.getInput(entry.member)
    if (!input?.connection) {
        return
    }
    Blockly.Events.setGroup(true)
    try {
        parent.setCollapsed(false)
        const block = create(parent.workspace as Blockly.WorkspaceSvg, entry.type)
        if (entry.memberKind === "value") {
            input.connection.connect(block.outputConnection!)
        } else {
            let last = input.connection.targetBlock()
            while (last?.getNextBlock()) {
                last = last.getNextBlock()
            }
            const target = last ? last.nextConnection! : input.connection
            target.connect(block.previousConnection!)
        }
        Blockly.common.setSelected(block)
    } finally {
        Blockly.Events.setGroup(false)
    }
}

function visible(entry: AddEntry, block: Blockly.BlockSvg | undefined) {
    if (!block || block.type !== entry.owner || !block.isEditable()) {
        return false
    }
    return entry.memberKind !== "value" || !block.getInput(entry.member)?.connection?.targetBlock()
}

export function registerAddMenu(schema: Schema) {
    const registry = Blockly.ContextMenuRegistry.registry
    const replace = (item: Blockly.ContextMenuRegistry.RegistryItem) => {
        if (registry.getItem(item.id)) {
            registry.unregister(item.id)
        }
        registry.register(item)
    }
    let weight = 200
    let group = ""
    for (const entry of schema.blocks.flatMap(owner => addEntries(schema, owner))) {
        if (group !== entry.owner + entry.member) {
            group = entry.owner + entry.member
            replace({
                id: `add_${entry.owner}_${entry.member}_separator`,
                scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
                separator: true,
                weight: weight++,
                preconditionFn: scope => visible(entry, scope.block) ? "enabled" : "hidden"
            })
        }
        replace({
            id: entry.id,
            scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
            displayText: entry.text,
            weight: weight++,
            preconditionFn: scope => visible(entry, scope.block) ? "enabled" : "hidden",
            callback: scope => scope.block && apply(entry, scope.block)
        })
    }
}
