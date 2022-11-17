import { AnQLBaseVisitor } from '@anql/core/dist'
import { Predicate } from '@src/types'

export class AnQLFunctionsVisitor<TModel> extends AnQLBaseVisitor<TModel, Predicate<TModel>> {
    successQueryResult: Predicate<TModel> = _ => true
    failedQueryResult: Predicate<TModel> = _ => false
    public handleAnd(left: Predicate<TModel>, right: Predicate<TModel>): Predicate<TModel> {
        return arg => left(arg) && right(arg);
    }
    public handleOr(left: Predicate<TModel>, right: Predicate<TModel>): Predicate<TModel> {
        return arg => left(arg) || right(arg)
    }
    public handleNot(childExpression: Predicate<TModel>): Predicate<TModel> {
        return arg => !childExpression(arg)
    }
    public handleAnyEqual(...childExpressions: Predicate<TModel>[]): Predicate<TModel> {
        return childExpressions.reduce(this.handleOr);
    }
}
