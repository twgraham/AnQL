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
    
    public abstract TReturn VisitExprAND(TReturn left, TReturn right);
    public abstract TReturn VisitExprOR(TReturn left, TReturn right);
    public abstract TReturn VisitExprNOT(TReturn childExpression);
    public abstract TReturn VisitAnyEqual(params TReturn[] childExpressions);

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

    public override TReturn VisitExprAND(AnQLGrammarParser.ExprANDContext context)
    {
        return VisitExprAND(Visit(context.expr(0)), Visit(context.expr(1)));
    }
    
    public override TReturn VisitExprOR(AnQLGrammarParser.ExprORContext context)
    {
        return VisitExprOR(Visit(context.expr(0)), Visit(context.expr(1)));
    }

    public override TReturn VisitNOT(AnQLGrammarParser.NOTContext context)
    {
        return VisitExprNOT(Visit(context.expr()));
    }

    public override TReturn VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    public override TReturn VisitEqual(AnQLGrammarParser.EqualContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.property_path(), context.value());
    }

    public override TReturn VisitGreaterThan(AnQLGrammarParser.GreaterThanContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.property_path(), context.value());
    }

    public override TReturn VisitLessThan(AnQLGrammarParser.LessThanContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.property_path(), context.value());
    }
    
    public override TReturn VisitAnyEqual(AnQLGrammarParser.AnyEqualContext context)
    {
        return VisitAnyEqual(context.value()
            .Select(valueContext => BuildFilter(QueryOperation.Equal, context.property_path(), valueContext))
            .ToArray());
    }

    protected virtual TReturn TransformFilter(TReturn filterExpression) => filterExpression;

    private TReturn BuildFilter(QueryOperation operation,
        AnQLGrammarParser.Property_pathContext propertyPathContext, AnQLGrammarParser.ValueContext valueContext)
    {
        if (!ResolverMap.TryGet(propertyPathContext.GetText(), out var resolver))
            return HandleUnknownProperty(propertyPathContext);

        var (value, type) = valueContext.GetValueAndAnQLType();

        return TransformFilter(resolver.Resolve(operation, value, type));
    }

    protected TReturn HandleUnknownProperty(AnQLGrammarParser.Property_pathContext propertyPathContext)
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
                case AnQLGrammarParser.ExprANDContext:
                    return notSwitch ? FailedQueryResult : SuccessQueryResult;
                case AnQLGrammarParser.ExprORContext:
                    return notSwitch ? SuccessQueryResult : FailedQueryResult;
                case AnQLGrammarParser.NOTContext:
                    notSwitch = !notSwitch;
                    break;
            }

            currentContext = currentContext.Parent;
        }

        return notSwitch ? FailedQueryResult : SuccessQueryResult;
    }
}