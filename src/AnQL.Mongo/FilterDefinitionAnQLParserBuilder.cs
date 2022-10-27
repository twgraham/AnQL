using AnQL.Core;
using MongoDB.Driver;

namespace AnQL.Mongo;

public class FilterDefinitionAnQLParserBuilder<T> : AnQLParserBuilder<FilterDefinition<T>, T>
{
    public FilterDefinitionAnQLParserBuilder(AnQLParserOptions options) : base(options)
    {
    }

    public override IAnQLParser<FilterDefinition<T>> Build()
    {
        return new AnQLParser<FilterDefinition<T>>(new AnQLFilterDefinitionVisitor<T>(ResolverMap, Options));
    }
}