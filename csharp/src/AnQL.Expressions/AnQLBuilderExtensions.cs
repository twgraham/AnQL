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
        var builder = new ExpressionAnQLParserBuilder<T>(options);
        
        builder.RegisterComparableType<ushort>()
            .RegisterComparableType<short>()
            .RegisterComparableType<uint>()
            .RegisterComparableType<int>()
            .RegisterComparableType<ulong>()
            .RegisterComparableType<long>()
            .RegisterComparableType<float>()
            .RegisterComparableType<double>()
            .RegisterComparableType<decimal>()
            .RegisterComparableType<string>();

        return builder;
    }
}