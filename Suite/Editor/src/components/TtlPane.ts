export class TtlPane extends HTMLElement {
    pre!: HTMLPreElement

    connectedCallback() {
        this.pre = document.createElement("pre")
        this.append(this.pre)
    }

    show(turtle: string) {
        this.pre.textContent = turtle
    }
}
