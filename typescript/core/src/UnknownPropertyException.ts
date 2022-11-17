export default class UnknownPropertyError extends Error {
    public readonly property: string

    constructor(property: string) {
        super()
        this.property = property
    }
}