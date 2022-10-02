using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Resolvers;
using AnQL.Expressions.Helpers;

namespace AnQL.Expressions;

internal class AnQLExpressionsVisitor<T> : AnQLGrammarBaseVisitor<Expression<Func<T, bool>>?>
{
    private static readonly ConstantExpression TrueConstant = Expression.Constant(true);
    private static readonly ConstantExpression FalseConstant = Expression.Constant(false);
    private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(T));
    private static readonly Expression<Func<T, bool>> TrueExpression = Expression.Lambda<Func<T, bool>>(TrueConstant, Parameter);
    private static readonly Expression<Func<T, bool>> FalseExpression = Expression.Lambda<Func<T, bool>>(FalseConstant, Parameter);

    private readonly Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> _resolverMap;
    
    public AnQLExpressionsVisitor(Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> resolverMap)
    {
        _resolverMap = resolverMap;
    }

    public override Expression<Func<T, bool>> VisitQuery(AnQLGrammarParser.QueryContext context)
    {
        return base.VisitQuery(context) ?? TrueExpression;
    }

    public override Expression<Func<T, bool>>? VisitExprAND(AnQLGrammarParser.ExprANDContext context)
    {
        // If either side of the expression is null, then we should evaluate it as a constant true.
        // This way the other side of the AND expression becomes determinant
        var left = Visit(context.expr(0)) ?? TrueExpression;
        var right = Visit(context.expr(1)) ?? TrueExpression;

        // If both sides evaluate to null, then return null
        if (left == TrueExpression && right == TrueExpression)
            return null;

        return left.Update(Expression.AndAlso(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>>? VisitExprOR(AnQLGrammarParser.ExprORContext context)
    {
        // If either side of the expression is null, then we should evaluate it as a constant false.
        // This way the other side of the OR expression becomes determinant
        var left = Visit(context.expr(0)) ?? FalseExpression;
        var right = Visit(context.expr(1)) ?? FalseExpression;

        // If both sides evaluate to null, then return null
        if (left == FalseExpression && right == FalseExpression)
            return null;

        return left.Update(Expression.OrElse(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>>? VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    public override Expression<Func<T, bool>>? VisitNOT(AnQLGrammarParser.NOTContext context)
    {
        var expr = Visit(context.expr());
        return expr == null ? null : Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);
    }

    public override Expression<Func<T, bool>> VisitEq(AnQLGrammarParser.EqContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitGt(AnQLGrammarParser.GtContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitLt(AnQLGrammarParser.LtContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitAnyEq(AnQLGrammarParser.AnyEqContext context)
    {
        return context.value()
            .Select(valueContext => BuildFilter(QueryOperation.Equal, context.property_path(), valueContext))
            .Aggregate((agg, curr) => agg.CombineExpression(curr, Expression.OrElse));
    }

    /// <summary>
    /// Build a filter definition from parse tree context and a given delagate.
    /// </summary>
    /// <param name="operation">Type of operation that was visited</param>
    /// <param name="propertyPathContext">Filter parse tree property context</param>
    /// <param name="valueContext">Filter parse tree value context</param>
    /// <returns>Filter definition</returns>
    private Expression<Func<T, bool>> BuildFilter(QueryOperation operation, AnQLGrammarParser.Property_pathContext propertyPathContext, AnQLGrammarParser.ValueContext valueContext)
    {
        _resolverMap.TryGetValue(propertyPathContext.GetText().ToLower(), out var resolver);

        if (resolver == null)
            return Expression.Lambda<Func<T, bool>>(Expression.Default(typeof(T)), Parameter);

        var (value, type) = GetNativeValue(valueContext);

        var resolvedExpression = resolver.Resolve(operation, value, type);
        return Expression.Lambda<Func<T, bool>>(resolvedExpression.Body.Replace(resolvedExpression.Parameters[0], Parameter), Parameter);
    }
    
    /// <summary>
    /// Get the native value from the text value in the parse tree value context.
    /// </summary>
    /// <param name="context">Value context in parse tree</param>
    /// <returns>A tuple of the native value and value type</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static (string, AnQLValueType) GetNativeValue(AnQLGrammarParser.ValueContext context)
    {
        var value = context.GetText();
        switch (context)
        {
            case AnQLGrammarParser.NullContext:
                return (value, AnQLValueType.Null);
            case AnQLGrammarParser.BoolContext:
                return (value, AnQLValueType.Bool);
            case AnQLGrammarParser.NumberContext:
                return (value, AnQLValueType.Number);
            case AnQLGrammarParser.StringContext:
                // Remove single quotes from start and end of string
                return (ExtractStringValue(value), AnQLValueType.String);
            default:
                throw new ArgumentOutOfRangeException();
        }

        string ExtractStringValue(string toExtract)
        {
            // First remove surrounding quotes
            var firstChar = toExtract[0];

            // If it doesn't start with a quote, return early
            if (firstChar != '\'' && firstChar != '"')
                return toExtract;

            var extractedString = toExtract.Substring(1, value.Length - 2);

            // Replace any escaped internal single quotes
            extractedString = extractedString.Replace(@"\'", "'");
            return extractedString;
        }
    }
}