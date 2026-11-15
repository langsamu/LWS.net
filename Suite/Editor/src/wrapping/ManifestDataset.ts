import { DataFactory } from "n3"
import type { BlankNode, NamedNode, Term } from "@rdfjs/types"
import { DatasetWrapper } from "@rdfjs/wrapper"
import { Vocabulary } from "../vocabulary/mod.ts"
import { Manifest } from "./Manifest.ts"

const { namedNode } = DataFactory

export class ManifestDataset extends DatasetWrapper {
    get root() {
        const subjects = new Map<string, Term>()
        for (const predicate of [Vocabulary.test, Vocabulary.include]) {
            for (const q of this.match(undefined, namedNode(predicate))) {
                subjects.set(q.subject.termType + q.subject.value, q.subject)
            }
        }
        if (subjects.size !== 1) {
            throw new Error(`Expected exactly one root manifest, found ${subjects.size}`)
        }
        return new Manifest([...subjects.values()][0] as NamedNode | BlankNode, this, DataFactory)
    }
}
