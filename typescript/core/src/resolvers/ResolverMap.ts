import { IAnQLPropertyResolver } from "./IAnQLPropertyResolver";

export default class ResolverMap<TReturn, TModel> {
    
    public get(property: string): IAnQLPropertyResolver<TReturn> {
        throw new Error()
    }
}