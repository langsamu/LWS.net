import { LiteralAs, NamedNodeAs, OptionalFrom, SetFrom, TermAs, TermFrom } from "@rdfjs/wrapper"
import type { Term } from "@rdfjs/types"
import { DASH, Editor, RDF, RDFS, SHACL } from "../vocabulary/mod.ts"
import { Described } from "./Described.ts"
import { PropertyShape } from "./PropertyShape.ts"

export class NodeShape extends Described {
    get name() {
        return OptionalFrom.subjectPredicate(this, SHACL.name, LiteralAs.string)
    }

    get isAbstract() {
        return OptionalFrom.subjectPredicate(this, DASH.abstract, LiteralAs.boolean) ?? false
    }

    get hue() {
        return OptionalFrom.subjectPredicate(this, Editor.hue, LiteralAs.number)
    }

    get properties() {
        return SetFrom.subjectPredicate(this, SHACL.property, TermAs.instance(PropertyShape), TermFrom.instance)
    }

    get superClasses() {
        return SetFrom.subjectPredicate(this, RDFS.subClassOf, TermAs.instance(NodeShape), TermFrom.instance)
    }

    get targetSubjectsOf() {
        return SetFrom.subjectPredicate(this, SHACL.targetSubjectsOf, NamedNodeAs.string, (value, factory) => factory.namedNode(value))
    }

    get alternatives() {
        return OptionalFrom.subjectPredicate(this, SHACL.xone, TermAs.list(this, SHACL.xone, TermAs.instance(NodeShape), TermFrom.instance))
    }

    get isClass() {
        const type = this.factory.namedNode(RDF.type)
        const klass = this.factory.namedNode(RDFS.Class)
        return !this.dataset.match(this as Term, type, klass)[Symbol.iterator]().next().done
    }

    get listItem(): NodeShape | undefined {
        for (const property of this.properties) {
            if (property.isListMembership) {
                return property.node
            }
        }
        return undefined
    }

    * subClasses() {
        const predicate = this.factory.namedNode(RDFS.subClassOf)
        for (const q of this.dataset.match(undefined, predicate, this as Term)) {
            yield new NodeShape(q.subject, this.dataset, this.factory)
        }
    }
}
