using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Extensions;
using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;
using AnQL.Expressions.Helpers;

namespace AnQL.Expressions;

public class AnQLExpressionsVisitor<T> : AnQLBaseVisitor<Expression<Func<T, bool>>>
{
    private static readonly ConstantExpression TrueConstant = Expression.Constant(true);
    private static readonly ConstantExpression FalseConstant = Expression.Constant(false);
    private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(T));
    private static readonly Expression<Func<T, bool>> TrueExpression = Expression.Lambda<Func<T, bool>>(TrueConstant, Parameter);
    private static readonly Expression<Func<T, bool>> FalseExpression = Expression.Lambda<Func<T, bool>>(FalseConstant, Parameter);

    private readonly Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> _resolverMap;

    public override Expression<Func<T, bool>> SuccessQueryResult => TrueExpression;
    public override Expression<Func<T, bool>> FailedQueryResult => FalseExpression;
    
    public AnQLExpressionsVisitor(Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> resolverMap, AnQLParserOptions options)
        : base(options)
    {
        _resolverMap = resolverMap;
    }

    public override Expression<Func<T, bool>> VisitExprAND(AnQLGrammarParser.ExprANDContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));

        return left.Update(Expression.AndAlso(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>> VisitExprOR(AnQLGrammarParser.ExprORContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));

        return left.Update(Expression.OrElse(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>> VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    public override Expression<Func<T, bool>> VisitNOT(AnQLGrammarParser.NOTContext context)
    {
        var expr = Visit(context.expr());
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);
    }

    public override Expression<Func<T, bool>> VisitEqual(AnQLGrammarParser.EqualContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitGreaterThan(AnQLGrammarParser.GreaterThanContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitLessThan(AnQLGrammarParser.LessThanContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.property_path(), context.value());
    }

    public override Expression<Func<T, bool>> VisitAnyEqual(AnQLGrammarParser.AnyEqualContext context)
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
            return HandleUnknownProperty(propertyPathContext);

        var (value, type) = valueContext.GetValueAndAnQLType();

        var resolvedExpression = resolver.Resolve(operation, value, type);
        return Expression.Lambda<Func<T, bool>>(resolvedExpression.Body.Replace(resolvedExpression.Parameters[0], Parameter), Parameter);
    }
}