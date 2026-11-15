import { LiteralAs, OptionalFrom, TermWrapper } from "@rdfjs/wrapper"
import { RDFS } from "../vocabulary/mod.ts"

export class Described extends TermWrapper {
    get label() {
        return OptionalFrom.subjectPredicate(this, RDFS.label, LiteralAs.string)
    }

    get comment() {
        return OptionalFrom.subjectPredicate(this, RDFS.comment, LiteralAs.string)
    }
}
