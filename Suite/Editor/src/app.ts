import dataset from "@rdfjs/dataset"
import { buildSchema } from "./schema/mod.ts"
import { datasetToState, parse, quadsToTurtle, stateToQuads, type State } from "./rdf.ts"
import { ManifestEditor } from "./components/ManifestEditor.ts"
import { TtlPane } from "./components/TtlPane.ts"

customElements.define("manifest-editor", ManifestEditor)
customElements.define("ttl-pane", TtlPane)

const editor = document.querySelector<ManifestEditor>("manifest-editor")!
const pane = document.querySelector<TtlPane>("ttl-pane")!
const download = document.querySelector<HTMLAnchorElement>("#download")!

const [vocabulary, shapes, manifest] = await Promise.all(
    ["../Model/Resources/vocabulary.ttl", "./shapes.ttl", "../Model/Resources/new.ttl"].map(async url => (await fetch(url)).text())
)
const schema = buildSchema(dataset.dataset([...parse(vocabulary), ...parse(shapes)]))
editor.init(schema)

function refresh(state: State) {
    const turtle = quadsToTurtle(stateToQuads(state, schema))
    pane.show(turtle)
    if (download.href) {
        URL.revokeObjectURL(download.href)
    }
    download.href = URL.createObjectURL(new Blob([turtle], { type: "text/turtle" }))
}

editor.addEventListener("state-changed", (event: Event) => refresh((event as CustomEvent<State>).detail))

const initial = datasetToState(parse(manifest), schema)
editor.load(initial)
refresh(initial)
