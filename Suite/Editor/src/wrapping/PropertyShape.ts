import { LiteralAs, NamedNodeAs, OptionalFrom, RequiredFrom, TermAs, TermFrom } from "@rdfjs/wrapper"
import type { Term } from "@rdfjs/types"
import { RDF, SHACL } from "../vocabulary/mod.ts"
import { Described } from "./Described.ts"
import { NodeShape } from "./NodeShape.ts"

export class PropertyShape extends Described {
    get path(): Term {
        return RequiredFrom.subjectPredicate(this, SHACL.path, TermAs.term)
    }

    get described(): Described | undefined {
        const path = this.path
        return path.termType === "NamedNode" ? new Described(path, this.dataset, this.factory) : undefined
    }

    get isListMembership() {
        const path = this.path
        return path.termType === "BlankNode" && !this.dataset.match(path, this.factory.namedNode(RDF.first))[Symbol.iterator]().next().done
    }

    get name() {
        return OptionalFrom.subjectPredicate(this, SHACL.name, LiteralAs.string)
    }

    get order() {
        return OptionalFrom.subjectPredicate(this, SHACL.order, LiteralAs.number)
    }

    get minCount() {
        return OptionalFrom.subjectPredicate(this, SHACL.minCount, LiteralAs.number)
    }

    get maxCount() {
        return OptionalFrom.subjectPredicate(this, SHACL.maxCount, LiteralAs.number)
    }

    get node() {
        return OptionalFrom.subjectPredicate(this, SHACL.node, TermAs.instance(NodeShape))
    }

    get hasValue() {
        return OptionalFrom.subjectPredicate(this, SHACL.hasValue, NamedNodeAs.string)
    }

    get datatypes(): string[] {
        const own = OptionalFrom.subjectPredicate(this, SHACL.datatype, NamedNodeAs.string)
        if (own !== undefined) {
            return [own]
        }
        const alternatives = OptionalFrom.subjectPredicate(this, SHACL.or, TermAs.list(this, SHACL.or, TermAs.instance(PropertyShape), TermFrom.instance))
        return [...(alternatives ?? [])].flatMap(alternative => alternative.datatypes)
    }
}
