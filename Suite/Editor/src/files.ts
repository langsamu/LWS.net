const TURTLE_TYPES = [{ description: "Turtle", accept: { "text/turtle": [".ttl"] } }]

interface FilePickers {
    showOpenFilePicker?(options: object): Promise<FileSystemFileHandle[]>
    showSaveFilePicker?(options: object): Promise<FileSystemFileHandle>
}

const pickers = window as unknown as FilePickers

export async function pickFile(): Promise<{ file: File, handle?: FileSystemFileHandle } | undefined> {
    if (pickers.showOpenFilePicker) {
        try {
            const [handle] = await pickers.showOpenFilePicker({ types: TURTLE_TYPES })
            return { file: await handle.getFile(), handle }
        } catch (error) {
            return cancelled(error)
        }
    }
    return new Promise(resolve => {
        const input = document.createElement("input")
        input.type = "file"
        input.accept = ".ttl,text/turtle"
        input.addEventListener("change", () => resolve(input.files?.[0] && { file: input.files[0] }))
        input.addEventListener("cancel", () => resolve(undefined))
        input.click()
    })
}

export async function write(handle: FileSystemFileHandle, text: string) {
    const writable = await handle.createWritable()
    await writable.write(text)
    await writable.close()
}

export const canPickSave = Boolean(pickers.showSaveFilePicker)

export async function pickSave(name: string) {
    try {
        return await pickers.showSaveFilePicker!({ suggestedName: name, types: TURTLE_TYPES })
    } catch (error) {
        return cancelled(error)
    }
}

export function download(name: string, text: string) {
    const link = document.createElement("a")
    link.href = URL.createObjectURL(new Blob([text], { type: "text/turtle" }))
    link.download = name
    link.click()
    URL.revokeObjectURL(link.href)
}

function cancelled(error: unknown) {
    if (error instanceof DOMException && error.name === "AbortError") {
        return undefined
    }
    throw error
}
