import dataset from "@rdfjs/dataset"
import { buildSchema } from "./schema/mod.ts"
import { datasetToState, emptyState, parse, quadsToTurtle, stateToQuads, type State } from "./rdf.ts"
import { ManifestEditor } from "./components/ManifestEditor.ts"
import { TtlPane } from "./components/TtlPane.ts"
import { FileToolbar } from "./components/FileToolbar.ts"
import { canPickSave, download, pickFile, pickSave, write } from "./files.ts"

customElements.define("manifest-editor", ManifestEditor)
customElements.define("ttl-pane", TtlPane)
customElements.define("file-toolbar", FileToolbar)

const editor = document.querySelector<ManifestEditor>("manifest-editor")!
const pane = document.querySelector<TtlPane>("ttl-pane")!
const toolbar = document.querySelector<FileToolbar>("file-toolbar")!

const sample = new URLSearchParams(location.search).get("file")
const [vocabulary, shapes] = await Promise.all(
    ["../Model/Resources/vocabulary.ttl", "./shapes.ttl"].map(async url => (await fetch(url)).text())
)
const schema = buildSchema(dataset.dataset([...parse(vocabulary), ...parse(shapes)]))
editor.init(schema)

let name = "manifest.ttl"
let handle: FileSystemFileHandle | undefined
let saved = ""
let turtle = ""

function update(state: State) {
    turtle = quadsToTurtle(stateToQuads(state, schema))
    pane.show(turtle)
}

function isDirty() {
    return turtle !== saved
}

function status() {
    toolbar.show(name, isDirty())
}

function markSaved() {
    saved = turtle
    status()
}

function load(state: State, fileName: string, fileHandle?: FileSystemFileHandle) {
    editor.load(state)
    name = fileName
    handle = fileHandle
    update(editor.state())
    markSaved()
}

function confirmDiscard() {
    return !isDirty() || confirm("Discard unsaved changes?")
}

async function openFile(file: File, fileHandle?: FileSystemFileHandle) {
    try {
        load(datasetToState(parse(await file.text()), schema), file.name, fileHandle)
    } catch (error) {
        alert(`Cannot open ${file.name}: ${error instanceof Error ? error.message : error}`)
    }
}

async function saveAs() {
    if (!canPickSave) {
        download(name, turtle)
        markSaved()
        return
    }
    const picked = await pickSave(name)
    if (picked) {
        await write(picked, turtle)
        handle = picked
        name = picked.name
        markSaved()
    }
}

async function save() {
    if (handle) {
        await write(handle, turtle)
        markSaved()
    } else {
        await saveAs()
    }
}

editor.addEventListener("state-changed", (event: Event) => {
    update((event as CustomEvent<State>).detail)
    status()
})

toolbar.addEventListener("new", () => {
    if (confirmDiscard()) {
        load(emptyState(schema), "manifest.ttl")
    }
})

toolbar.addEventListener("open", async () => {
    if (confirmDiscard()) {
        const picked = await pickFile()
        if (picked) {
            await openFile(picked.file, picked.handle)
        }
    }
})

toolbar.addEventListener("save", save)
toolbar.addEventListener("save-as", saveAs)

addEventListener("dragover", event => event.preventDefault())

addEventListener("drop", async event => {
    event.preventDefault()
    const item = event.dataTransfer?.items[0]
    const file = item?.getAsFile()
    if (file && file.name.endsWith(".ttl") && confirmDiscard()) {
        const fileHandle = await (item as { getAsFileSystemHandle?(): Promise<FileSystemHandle | null> }).getAsFileSystemHandle?.()
        await openFile(file, fileHandle?.kind === "file" ? fileHandle as FileSystemFileHandle : undefined)
    }
})

addEventListener("beforeunload", event => {
    if (isDirty()) {
        event.preventDefault()
    }
})

const initial = sample ?? "new.ttl"
load(datasetToState(parse(await (await fetch(`../Model/Resources/${initial}`)).text()), schema), initial)
