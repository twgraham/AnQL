using System.Linq.Expressions;

namespace AnQL.Expressions.Helpers;

internal static class ExpressionHelper
{
    public static Expression<Func<TValue, TResult>> CombineExpression<TValue, TResult>(
        this Expression<Func<TValue, TResult>> left,
        Expression<Func<TValue, TResult>> right,
        Func<Expression, Expression, BinaryExpression> combination)
    {
        return Combine(left, right, combination);
    }

    public static Expression<Func<TValue, TResult>> Combine<TValue, TResult>(
        Expression<Func<TValue, TResult>> left,
        Expression<Func<TValue, TResult>> right,
        Func<Expression, Expression, BinaryExpression> combination)
    {
        // rewrite the body of "right" using "left"'s parameter in place
        // of the original "right"'s parameter
        var newRight = new ExpressionReplacer(right.Parameters[0], left.Parameters[0])
            .Visit(right.Body);
        // combine via && / || etc and create a new lambda
        return Expression.Lambda<Func<TValue, TResult>>(
            combination(left.Body, newRight), left.Parameters);
    }
    
    public static Expression Replace(this Expression expression, Expression source, Expression target)
    {
        return new ExpressionReplacer(source, target).Visit(expression);
    }
}