import { LiteralAs, LiteralFrom, RequiredFrom, SetFrom, TermWrapper } from "@rdfjs/wrapper"
import { Vocabulary } from "../vocabulary/mod.ts"

export class Manifest extends TermWrapper {
    get manifestName() {
        return RequiredFrom.subjectPredicate(this, Vocabulary.manifestName, LiteralAs.string)
    }

    get include() {
        return SetFrom.subjectPredicate(this, Vocabulary.include, LiteralAs.string, LiteralFrom.anyUriString)
    }
}
