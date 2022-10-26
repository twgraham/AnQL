using System.Linq.Expressions;
using AnQL.Core;

namespace AnQL.Expressions;

public static class AnQLBuilderExtensions
{
    public static ExpressionAnQLParserBuilder<T> ForExpressions<T>(this AnQLBuilder anqlBuilder)
    {
        return anqlBuilder.For<ExpressionAnQLParserBuilder<T>, Expression<Func<T, bool>>, T>(Create<T>);
    }

    private static ExpressionAnQLParserBuilder<T> Create<T>(AnQLParserOptions options)
    {
        return new ExpressionAnQLParserBuilder<T>(options);
    }
}