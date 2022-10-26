using AnQL.Core;
using AnQL.Functions.Resolvers;

namespace AnQL.Functions;

public static class AnQLBuilderExtensions
{
    public static FunctionAnQLParserBuilder<T> ForFunctions<T>(this AnQLBuilder anQlBuilder)
    {
        return anQlBuilder.For<FunctionAnQLParserBuilder<T>, Func<T, bool>, T>(Create<T>);
    }

    private static FunctionAnQLParserBuilder<T> Create<T>(AnQLParserOptions options)
    {
        var builder = new FunctionAnQLParserBuilder<T>(options);

        builder.RegisterComparableType<ushort>()
            .RegisterComparableType<short>()
            .RegisterComparableType<uint>()
            .RegisterComparableType<int>()
            .RegisterComparableType<ulong>()
            .RegisterComparableType<long>()
            .RegisterComparableType<float>()
            .RegisterComparableType<double>()
            .RegisterComparableType<decimal>();
        
        builder.RegisterFactory(typeof(string), new StringResolver<T>.Factory());

        return builder;
    }
}