const COMMANDS = [
    ["new", "New"],
    ["open", "Open…"],
    ["save", "Save"],
    ["save-as", "Save as…"]
] as const

export class FileToolbar extends HTMLElement {
    private label = document.createElement("span")

    connectedCallback() {
        for (const [command, text] of COMMANDS) {
            const button = document.createElement("button")
            button.type = "button"
            button.textContent = text
            button.addEventListener("click", () => this.dispatchEvent(new CustomEvent(command)))
            this.append(button)
        }
        this.append(this.label)
    }

    show(name: string, dirty: boolean) {
        this.label.textContent = dirty ? `${name} •` : name
        document.title = `${dirty ? "• " : ""}${name} - LWS manifest editor`
    }
}
