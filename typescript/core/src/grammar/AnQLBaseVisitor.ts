import { AbstractParseTreeVisitor } from 'antlr4ts/tree'
import type AnQLParserOptions from '@src/AnQLParserOptions'
import { AnQLGrammarLexer } from '@gen/grammar/AnQLGrammarLexer'
import { AndContext, AnyEqualContext, BoolContext, EqualContext, FreeTextContext, GreaterThanContext, LessThanContext, NotContext, NullContext, NumberContext, OrContext, PropertyPathContext, StringContext, ValueContext } from '@gen/grammar/AnQLGrammarParser'
import type IAnQLVisitor from '@src/grammar/IAnQLVisitor'
import AnQLValueType from '@src/resolvers/AnQLValueType'
import QueryOperation from '@src/resolvers/QueryOperation'
import ResolverMap from '@src/resolvers/ResolverMap'
import UnknownPropertyBehaviour from '@src/UnknownPropertyBehaviour'
import UnknownPropertyError from '@src/UnknownPropertyException'

export abstract class AnQLBaseVisitor<TModel, TReturn> extends AbstractParseTreeVisitor<TReturn> implements IAnQLVisitor<TReturn> {
    private readonly resolverMap: ResolverMap<TReturn, TModel>;
    private readonly options: AnQLParserOptions;

    abstract successQueryResult: TReturn
    abstract failedQueryResult: TReturn

    constructor(resolverMap: ResolverMap<TReturn, TModel>, options: AnQLParserOptions) {
        super()
        this.resolverMap = resolverMap;
        this.options = options;
    }

    public abstract handleAnd(left: TReturn, right: TReturn): TReturn
    public abstract handleOr(left: TReturn, right: TReturn): TReturn
    public abstract handleNot(childExpression: TReturn): TReturn
    public abstract handleAnyEqual(...childExpressions: TReturn[]): TReturn;

    public visitAnd(ctx: AndContext): TReturn {
        return this.handleAnd(this.visit(ctx._left), this.visit(ctx._right))
    }

    public visitOr(ctx: OrContext): TReturn {
        return this.handleOr(this.visit(ctx._left), this.visit(ctx._right))
    }

    public visitNot(ctx: NotContext): TReturn {
        return this.handleNot(this.visit(ctx.expr()))
    }

    public visitEqual(context: EqualContext): TReturn {
        return this.buildFilter(QueryOperation.Equal, context.propertyPath(), context.value())
    }

    public visitAnyEqual(ctx: AnyEqualContext): TReturn {
        return this.handleAnyEqual(...ctx.value().map(x => this.buildFilter(QueryOperation.Equal, ctx.propertyPath(), x)))
    }

    public visitGreaterThan(context: GreaterThanContext): TReturn {
        return this.buildFilter(QueryOperation.GreaterThan, context.propertyPath(), context.value())
    }

    public visitLessThan(context: LessThanContext): TReturn {
        return this.buildFilter(QueryOperation.LessThan, context.propertyPath(), context.value())
    }

    public visitFreeText(ctx: FreeTextContext): TReturn {
        return this.successQueryResult
    }

    public transformFilter(expression: TReturn) {
        return expression
    }

    private buildFilter(operation: QueryOperation, propertyPathContext: PropertyPathContext, valueContext: ValueContext) {
        const resolver = this.resolverMap.get(propertyPathContext.text)
        if (!resolver)
            return this.handleUnknownProperty(propertyPathContext)

        var { value, valueType } = getValueAndAnQLType(valueContext)

        return this.transformFilter(resolver.resolve(operation, value, valueType))
    }

    protected handleUnknownProperty(propertyPathContext: PropertyPathContext): TReturn
    {
        if (this.options.unknownPropertyBehaviour === UnknownPropertyBehaviour.Throw
            || this.options.unknownPropertyBehaviour === UnknownPropertyBehaviour.Fail)
            throw new UnknownPropertyError(propertyPathContext.text);

        if (this.options.unknownPropertyBehaviour === UnknownPropertyBehaviour.Pass)
            return this.successQueryResult;

        let currentContext = propertyPathContext.parent?.parent;
        let notSwitch = false;
        while (currentContext != null)
        {
            if (currentContext instanceof AndContext) {
                return notSwitch ? this.failedQueryResult : this.successQueryResult;
            }

            if (currentContext instanceof OrContext) {
                return notSwitch ? this.successQueryResult : this.failedQueryResult;
            }

            if (currentContext instanceof NotContext) {
                notSwitch = !notSwitch;
            }

            currentContext = currentContext.parent;
        }

        return notSwitch ? this.failedQueryResult : this.successQueryResult;
    }

    protected defaultResult(): TReturn {
        return this.failedQueryResult
    }
}

function getValueAndAnQLType(valueContext: ValueContext): { value: string, valueType: AnQLValueType }
{
    const value: string = valueContext.text;

    if (valueContext instanceof NullContext) {
        return { value, valueType: AnQLValueType.Null }
    }

    if (valueContext instanceof BoolContext) {
        return { value, valueType: AnQLValueType.Bool }
    }

    if (valueContext instanceof NumberContext) {
        return { value, valueType: AnQLValueType.Number }
    }

    if (valueContext instanceof StringContext) {
        const val = valueContext.getToken(AnQLGrammarLexer.Quote, 0) != null ? value.slice(1, -1) : value
        return { value: val, valueType: AnQLValueType.String }
    }

    throw new Error('Unknown value context')
}
