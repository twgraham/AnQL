using System.Linq.Expressions;

namespace AnQL.Core.Resolvers;

public interface IResolverFactory<TModel, out TReturn>
{
    IAnQLPropertyResolver<TReturn> Build(Expression<Func<TModel, object>> propertyPath);
}