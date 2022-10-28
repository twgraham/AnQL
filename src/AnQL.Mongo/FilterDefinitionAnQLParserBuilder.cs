using AnQL.Core;
using AnQL.Mongo.Resolvers;
using MongoDB.Driver;

namespace AnQL.Mongo;

public class FilterDefinitionAnQLParserBuilder<T> : AnQLParserBuilder<FilterDefinition<T>, T>
{
    public FilterDefinitionAnQLParserBuilder(AnQLParserOptions options) : base(options)
    {
    }
    
    public FilterDefinitionAnQLParserBuilder<T> RegisterSimpleType<TType>()
    {
        return (FilterDefinitionAnQLParserBuilder<T>)RegisterFactory(typeof(TType),
            new SimpleResolver<T, TType>.Factory());
    }

    public override IAnQLParser<FilterDefinition<T>> Build()
    {
        return new AnQLParser<FilterDefinition<T>>(new AnQLFilterDefinitionVisitor<T>(ResolverMap, Options));
    }
}