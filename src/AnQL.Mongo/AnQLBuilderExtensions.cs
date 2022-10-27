using AnQL.Core;
using MongoDB.Driver;

namespace AnQL.Mongo;

public static class AnQLBuilderExtensions
{
    public static FilterDefinitionAnQLParserBuilder<T> ForFilterDefinitions<T>(this AnQLBuilder anQlBuilder)
    {
        return anQlBuilder.For<FilterDefinitionAnQLParserBuilder<T>, FilterDefinition<T>, T>(Create<T>);
    }

    private static FilterDefinitionAnQLParserBuilder<T> Create<T>(AnQLParserOptions options)
    {
        return new FilterDefinitionAnQLParserBuilder<T>(options);
    }
}