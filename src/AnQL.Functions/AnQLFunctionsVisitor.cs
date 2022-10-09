using AnQL.Core;
using AnQL.Core.Extensions;
using AnQL.Core.Resolvers;

namespace AnQL.Functions;

public class AnQLFunctionsVisitor<T> : AnQLGrammarBaseVisitor<Func<T, bool>?>
{
    private static readonly Func<T, bool> AlwaysTrue = _ => true;
    private static readonly Func<T, bool> AlwaysFalse = _ => false;

    private readonly Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> _resolverMap;

    public AnQLFunctionsVisitor(Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> resolverMap)
    {
        _resolverMap = resolverMap;
    }

    public override Func<T, bool> VisitQuery(AnQLGrammarParser.QueryContext context)
    {
        return base.VisitQuery(context) ?? AlwaysFalse;
    }

    public override Func<T, bool>? VisitExprAND(AnQLGrammarParser.ExprANDContext context)
    {
        var left = Visit(context.expr(0)) ?? AlwaysTrue;
        var right = Visit(context.expr(1)) ?? AlwaysTrue;
        
        if (left == AlwaysTrue && right == AlwaysTrue)
            return null;

        return value => left(value) && right(value);
    }
    
    public override Func<T, bool>? VisitExprOR(AnQLGrammarParser.ExprORContext context)
    {
        var left = Visit(context.expr(0)) ?? AlwaysFalse;
        var right = Visit(context.expr(1)) ?? AlwaysFalse;
        
        if (left == AlwaysFalse && right == AlwaysFalse)
            return null;

        return value => left(value) || right(value);
    }

    public override Func<T, bool>? VisitNOT(AnQLGrammarParser.NOTContext context)
    {
        var inner = Visit(context.expr());

        if (inner == null)
            return null;

        return value => !inner(value);
    }

    public override Func<T, bool>? VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }
    
    public override Func<T, bool> VisitEqual(AnQLGrammarParser.EqualContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.property_path(), context.value());
    }

    public override Func<T, bool> VisitGreaterThan(AnQLGrammarParser.GreaterThanContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.property_path(), context.value());
    }

    public override Func<T, bool> VisitLessThan(AnQLGrammarParser.LessThanContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.property_path(), context.value());
    }

    public override Func<T, bool> VisitAnyEqual(AnQLGrammarParser.AnyEqualContext context)
    {
        return context.value()
            .Select(valueContext => BuildFilter(QueryOperation.Equal, context.property_path(), valueContext))
            .Aggregate((left, right) => value => left(value) || right(value));
    }

    private Func<T, bool> BuildFilter(QueryOperation operation, AnQLGrammarParser.Property_pathContext propertyPathContext, AnQLGrammarParser.ValueContext valueContext)
    {
        _resolverMap.TryGetValue(propertyPathContext.GetText(), out var resolver);

        if (resolver == null)
            return AlwaysFalse;

        var (value, type) = valueContext.GetValueAndAnQLType();

        return resolver.Resolve(operation, value, type);
    }
}