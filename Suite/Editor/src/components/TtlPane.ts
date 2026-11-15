import { EditorState } from "@codemirror/state"
import { EditorView, lineNumbers } from "@codemirror/view"
import { StreamLanguage, defaultHighlightStyle, syntaxHighlighting } from "@codemirror/language"
import { turtle } from "@codemirror/legacy-modes/mode/turtle"

export class TtlPane extends HTMLElement {
    view!: EditorView

    connectedCallback() {
        this.view = new EditorView({
            parent: this,
            state: EditorState.create({
                extensions: [
                    EditorState.readOnly.of(true),
                    EditorView.editable.of(false),
                    lineNumbers(),
                    EditorView.lineWrapping,
                    StreamLanguage.define(turtle),
                    syntaxHighlighting(defaultHighlightStyle),
                    EditorView.theme({
                        "&": { height: "100%" },
                        ".cm-scroller": { overflow: "auto" }
                    })
                ]
            })
        })
    }

    show(turtle: string) {
        this.view.dispatch({ changes: { from: 0, to: this.view.state.doc.length, insert: turtle } })
    }
}
