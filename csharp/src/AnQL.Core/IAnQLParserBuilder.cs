using System.Linq.Expressions;
using AnQL.Core.Resolvers;

namespace AnQL.Core;

public interface IAnQLParserBuilder<TReturn, TItem>
{
    IAnQLParserBuilder<TReturn, TItem> WithProperty(Expression<Func<TItem, object>> propertyPath);
    
    IAnQLParserBuilder<TReturn, TItem> WithProperty(string name, Expression<Func<TItem, object>> propertyPath);

    IAnQLParserBuilder<TReturn, TItem> WithProperty(string name, IAnQLPropertyResolver<TReturn> propertyResolver);

    IAnQLParserBuilder<TReturn, TItem> RegisterFactory<TType, TFactory>(TFactory factory)
        where TFactory : IResolverFactory<TItem, TReturn>;

    IAnQLParserBuilder<TReturn, TItem> RegisterFactory(Type type, IResolverFactory<TItem, TReturn> resolverType);

    IAnQLParserBuilder<TReturn, TItem> RegisterAllProperties();
    IAnQLParserBuilder<TReturn, TItem> RegisterTaggedProperties();

    IAnQLParser<TReturn> Build();
}