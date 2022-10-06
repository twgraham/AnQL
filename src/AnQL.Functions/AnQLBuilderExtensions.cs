using AnQL.Core;

namespace AnQL.Functions;

public static class AnQLBuilderExtensions
{
    public static FunctionAnQLParserBuilder<T> ForFunctions<T>(this AnQLBuilder anQlBuilder)
    {
        return anQlBuilder.For<FunctionAnQLParserBuilder<T>, Func<T, bool>?, T>(Create<T>);
    }
    
    private static FunctionAnQLParserBuilder<T> Create<T>(AnQLParserOptions options)
    {
        return new FunctionAnQLParserBuilder<T>(options);
    }
}