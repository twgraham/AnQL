using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;
using AnQL.Expressions.Helpers;
using static AnQL.Expressions.Constants;

namespace AnQL.Expressions;

public class AnQLExpressionsVisitor<T> : AnQLBaseVisitor<T, Expression<Func<T, bool>>>
{
    private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(T), "x");
    private static readonly Expression<Func<T, bool>> TrueExpression = Expression.Lambda<Func<T, bool>>(TrueConstantExpression, Parameter);
    private static readonly Expression<Func<T, bool>> FalseExpression = Expression.Lambda<Func<T, bool>>(FalseConstantExpression, Parameter);
    
    public override Expression<Func<T, bool>> SuccessQueryResult => TrueExpression;
    public override Expression<Func<T, bool>> FailedQueryResult => FalseExpression;

    public AnQLExpressionsVisitor(ResolverMap<Expression<Func<T, bool>>, T> resolverMap, AnQLParserOptions options)
        : base(resolverMap, options)
    {
    }
    
    public override Expression<Func<T, bool>> VisitExprAND(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return left.Update(Expression.AndAlso(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>> VisitExprOR(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return left.Update(Expression.OrElse(left.Body, right.Body), new []{Parameter});
    }

    public override Expression<Func<T, bool>> VisitExprNOT(Expression<Func<T, bool>> childExpression)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.Not(childExpression.Body), childExpression.Parameters);
    }
    
    public override Expression<Func<T, bool>> VisitAnyEqual(params Expression<Func<T, bool>>[] childExpressions)
    {
        return childExpressions.Aggregate((agg, curr) => agg.CombineExpression(curr, Expression.OrElse));
    }

    protected override Expression<Func<T, bool>> TransformFilter(Expression<Func<T, bool>> filterExpression)
    {
        return Expression.Lambda<Func<T, bool>>(filterExpression.Body.Replace(filterExpression.Parameters[0], Parameter), Parameter);
    }
}