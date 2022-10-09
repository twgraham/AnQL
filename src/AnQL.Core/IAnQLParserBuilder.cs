using System.Linq.Expressions;
using AnQL.Core.Resolvers;

namespace AnQL.Core;

public interface IAnQLParserBuilder<TReturn, TItem>
{
    IAnQLParserBuilder<TReturn, TItem> WithProperty<TValue>(Expression<Func<TItem, TValue>> propertyPath);

    IAnQLParserBuilder<TReturn, TItem> WithProperty(string name, IAnQLPropertyResolver<TReturn> propertyResolver);

    IAnQLParser<TReturn> Build();
}