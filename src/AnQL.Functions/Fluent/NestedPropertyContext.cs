using System.Linq.Expressions;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;
using AnQL.Functions.Resolvers;

namespace AnQL.Functions.Fluent;

public class NestedPropertyContext<T>
{
    private readonly Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> _resolverMap = new();

    internal NestedPropertyContext()
    {
    }
    
    public NestedPropertyContext<T> WithValueProperty<TItem>(Expression<Func<T, TItem>> propertyPath,
        Action<ValueTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return WithValueProperty(propertyName, propertyAccessor, configureOptions);
    }

    public NestedPropertyContext<T> WithValueProperty<TItem>(string name, Func<T, TItem> propertyAccessor,
        Action<ValueTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        _resolverMap.Add(name, new ValueTypeResolver<T, TItem>(propertyAccessor, configureOptions));
        return this;
    }

    internal Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> Build()
    {
        return _resolverMap;
    }
}