import { datasetToState, parse, quadsToTurtle, stateToQuads, type State } from "./rdf.ts"
import { ManifestEditor } from "./components/ManifestEditor.ts"
import { TtlPane } from "./components/TtlPane.ts"

customElements.define("manifest-editor", ManifestEditor)
customElements.define("ttl-pane", TtlPane)

const editor = document.querySelector<ManifestEditor>("manifest-editor")!
const pane = document.querySelector<TtlPane>("ttl-pane")!
const download = document.querySelector<HTMLAnchorElement>("#download")!

function refresh(state: State) {
    const turtle = quadsToTurtle(stateToQuads(state))
    pane.show(turtle)
    if (download.href) {
        URL.revokeObjectURL(download.href)
    }
    download.href = URL.createObjectURL(new Blob([turtle], { type: "text/turtle" }))
}

editor.addEventListener("state-changed", (event: Event) => refresh((event as CustomEvent<State>).detail))

const text = await (await fetch("../Model/Resources/new.ttl")).text()
const initial = datasetToState(parse(text))
editor.load(initial)
refresh(initial)
