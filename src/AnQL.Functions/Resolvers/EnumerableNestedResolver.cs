using AnQL.Core.Resolvers;

namespace AnQL.Functions.Resolvers;

public class EnumerableNestedResolver<T, TCollection> : IAnQLPropertyResolver<Func<T, bool>>
{
    private readonly Func<T, IEnumerable<TCollection>> _collectionAccessor;
    private readonly IAnQLPropertyResolver<Func<TCollection, bool>> _nestedResolver;

    public EnumerableNestedResolver(Func<T, IEnumerable<TCollection>> collectionAccessor, IAnQLPropertyResolver<Func<TCollection, bool>> nestedResolver)
    {
        _collectionAccessor = collectionAccessor;
        _nestedResolver = nestedResolver;
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        var itemPredicate = _nestedResolver.Resolve(op, value, valueType);
        return arg =>
        {
            var collection = _collectionAccessor(arg);
            return collection.Any(itemPredicate);
        };
    }
}