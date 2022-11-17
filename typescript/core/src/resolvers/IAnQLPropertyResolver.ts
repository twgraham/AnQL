import AnQLValueType from './AnQLValueType'
import QueryOperation from './QueryOperation'

export interface IAnQLPropertyResolver<T> {
    resolve(op: QueryOperation, value: string, valueType: AnQLValueType): T
}