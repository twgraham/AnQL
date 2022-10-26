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
    
    public NestedPropertyContext<T> WithProperty<TItem>(Expression<Func<T, TItem>> propertyPath,
        Action<ComparableTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return WithProperty(propertyName, propertyAccessor, configureOptions);
    }

    public NestedPropertyContext<T> WithProperty<TItem>(string name, Func<T, TItem> propertyAccessor,
        Action<ComparableTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        _resolverMap.Add(name, new ComparableTypeResolver<T, TItem>(propertyAccessor, configureOptions));
        return this;
    }
    
    public NestedPropertyContext<T> WithProperty(Expression<Func<T, string>> propertyPath,
        Action<StringResolver<T>.Options>? configureOptions = null)
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return WithProperty(propertyName, propertyAccessor, configureOptions);
    }
    
    public NestedPropertyContext<T> WithProperty(string name, Func<T, string> propertyAccessor,
        Action<StringResolver<T>.Options>? configureOptions = null)
    {
        _resolverMap.Add(name, new StringResolver<T>(propertyAccessor, configureOptions));
        return this;
    }

    internal Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> Build()
    {
        return _resolverMap;
    }
}