import { datasetToState, parse, quadsToTurtle, stateToQuads, type State } from "./rdf.ts"
import { ManifestEditor } from "./components/ManifestEditor.ts"
import { TtlPane } from "./components/TtlPane.ts"

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
