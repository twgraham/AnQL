using AnQL.Core;
using AnQL.Mongo.Resolvers;
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
        var builder = new FilterDefinitionAnQLParserBuilder<T>(options);

        builder.RegisterFactory(typeof(string), new StringResolver<T>.Factory());
        builder.RegisterSimpleType<ushort>()
            .RegisterSimpleType<short>()
            .RegisterSimpleType<uint>()
            .RegisterSimpleType<int>()
            .RegisterSimpleType<ulong>()
            .RegisterSimpleType<long>()
            .RegisterSimpleType<float>()
            .RegisterSimpleType<double>()
            .RegisterSimpleType<decimal>()
            .RegisterSimpleType<DateTime>()
            .RegisterSimpleType<DateTimeOffset>()
            .RegisterSimpleType<DateOnly>()
            .RegisterSimpleType<bool>();

        return builder;
    }
}