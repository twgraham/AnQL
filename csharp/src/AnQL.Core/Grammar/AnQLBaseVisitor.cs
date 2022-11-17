using AnQL.Core.Extensions;
using AnQL.Core.Resolvers;

namespace AnQL.Core.Grammar;

public abstract class AnQLBaseVisitor<TModel, TReturn> : AnQLGrammarBaseVisitor<TReturn>, IAnQLVisitor<TReturn>
{
    protected ResolverMap<TReturn, TModel> ResolverMap { get; }
    protected AnQLParserOptions Options { get; }
    public abstract TReturn SuccessQueryResult { get; }
    public abstract TReturn FailedQueryResult { get; }

    protected AnQLBaseVisitor(ResolverMap<TReturn, TModel> resolverMap, AnQLParserOptions options)
    {
        ResolverMap = resolverMap;
        Options = options;
    }
    
    public abstract TReturn VisitAnd(TReturn left, TReturn right);
    public abstract TReturn VisitOr(TReturn left, TReturn right);
    public abstract TReturn VisitNot(TReturn childExpression);

    public virtual TReturn VisitAnyEqual(params TReturn[] childExpressions)
        => childExpressions.Aggregate(VisitOr);

    public override TReturn VisitQuery(AnQLGrammarParser.QueryContext context)
    {
        try
        {
            return base.VisitQuery(context);
        }
        catch (UnknownPropertyException)
        {
            if (Options.UnknownPropertyBehaviour == UnknownPropertyBehaviour.Throw)
                throw;
        }

        return FailedQueryResult;
    }

    public override TReturn VisitAnd(AnQLGrammarParser.AndContext context)
    {
        return VisitAnd(Visit(context.left), Visit(context.right));
    }
    
    public override TReturn VisitOr(AnQLGrammarParser.OrContext context)
    {
        return VisitOr(Visit(context.left), Visit(context.right));
    }

    public override TReturn VisitNot(AnQLGrammarParser.NotContext context)
    {
        return VisitNot(Visit(context.expr()));
    }

    public override TReturn VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    public override TReturn VisitEqual(AnQLGrammarParser.EqualContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.propertyPath(), context.value());
    }

    public override TReturn VisitGreaterThan(AnQLGrammarParser.GreaterThanContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.propertyPath(), context.value());
    }

    public override TReturn VisitLessThan(AnQLGrammarParser.LessThanContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.propertyPath(), context.value());
    }
    
    public override TReturn VisitAnyEqual(AnQLGrammarParser.AnyEqualContext context)
    {
        return VisitAnyEqual(context.value()
            .Select(valueContext => BuildFilter(QueryOperation.Equal, context.propertyPath(), valueContext))
            .ToArray());
    }

    protected virtual TReturn TransformFilter(TReturn filterExpression) => filterExpression;

    private TReturn BuildFilter(QueryOperation operation,
        AnQLGrammarParser.PropertyPathContext propertyPathContext, AnQLGrammarParser.ValueContext valueContext)
    {
        if (!ResolverMap.TryGet(propertyPathContext.GetText(), out var resolver))
            return HandleUnknownProperty(propertyPathContext);

        var (value, type) = valueContext.GetValueAndAnQLType();

        return TransformFilter(resolver.Resolve(operation, value, type));
    }

    protected TReturn HandleUnknownProperty(AnQLGrammarParser.PropertyPathContext propertyPathContext)
    {
        if (Options.UnknownPropertyBehaviour is UnknownPropertyBehaviour.Throw or UnknownPropertyBehaviour.Fail)
            throw new UnknownPropertyException(propertyPathContext.GetText());

        if (Options.UnknownPropertyBehaviour is UnknownPropertyBehaviour.Pass)
            return SuccessQueryResult;

        var currentContext = propertyPathContext.Parent.Parent;
        var notSwitch = false;
        while (currentContext != null)
        {
            switch (currentContext)
            {
                case AnQLGrammarParser.AndContext:
                    return notSwitch ? FailedQueryResult : SuccessQueryResult;
                case AnQLGrammarParser.OrContext:
                    return notSwitch ? SuccessQueryResult : FailedQueryResult;
                case AnQLGrammarParser.NotContext:
                    notSwitch = !notSwitch;
                    break;
            }

            currentContext = currentContext.Parent;
        }

        return notSwitch ? FailedQueryResult : SuccessQueryResult;
    }
}