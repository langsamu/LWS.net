import { DatasetWrapper } from "@rdfjs/wrapper"
import { SHACL } from "../vocabulary/mod.ts"
import { NodeShape } from "./NodeShape.ts"

export class ShapesDataset extends DatasetWrapper {
    get shapes() {
        return this.instancesOf(SHACL.NodeShape, NodeShape)
    }

    get root() {
        const roots = [...new Map([...this.subjectsOf(SHACL.targetSubjectsOf, NodeShape)].map(shape => [shape.value, shape])).values()]
        if (roots.length !== 1) {
            throw new Error(`Expected exactly one root shape, found ${roots.length}`)
        }
        return roots[0]
    }
}
