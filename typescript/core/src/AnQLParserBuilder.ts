import { IAnQLPropertyResolver } from "@src/resolvers/IAnQLPropertyResolver"
import type { Func } from "@src/types"
import AnQLParser from '@src/AnQLParser'

export interface IAnQLParserBuilder<TReturn, TModel> {
    withNamedProperty(name: string, resolver: IAnQLPropertyResolver<TReturn>): IAnQLParserBuilder<TReturn, TModel>
    withProperty<TValue>(accessor: Func<TModel, TValue>): IAnQLParserBuilder<TReturn, TModel>
}

abstract class AnQLParserBuilder<TReturn, TModel> implements IAnQLParserBuilder<TReturn, TModel> {
    private readonly resolverMap: Record<string, IAnQLPropertyResolver<TReturn>> = {}

    public abstract build(): AnQLParser<TReturn>;

    withNamedProperty(name: string, resolver: IAnQLPropertyResolver<TReturn>): IAnQLParserBuilder<TReturn, TModel> {
        this.resolverMap[name] = resolver
        return this
    }

    withProperty<TValue>(accessor: Func<TModel, TValue>): IAnQLParserBuilder<TReturn, TModel> {
        throw new Error('Method not implemented.')
    }
}
