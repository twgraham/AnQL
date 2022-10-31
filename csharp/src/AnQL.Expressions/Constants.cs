using System.Linq.Expressions;

namespace AnQL.Expressions;

public static class Constants
{
    public static readonly ConstantExpression TrueConstantExpression = Expression.Constant(true);
    public static readonly ConstantExpression FalseConstantExpression = Expression.Constant(false);
}