using System.Linq.Expressions;
using System.Reflection;
using AnQL.Core.Attributes;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;

namespace AnQL.Core;

public abstract class AnQLParserBuilder<TReturn, TItem> : IAnQLParserBuilder<TReturn, TItem>
{
    private readonly ResolverMap<TReturn, TItem> _resolverMap = new();

    protected AnQLParserOptions Options { get; }

    protected ResolverMap<TReturn, TItem> ResolverMap
    {
        get
        {
            _resolverMap.Freeze();
            return _resolverMap;
        }
    }

    protected AnQLParserBuilder(AnQLParserOptions options)
    {
        Options = options;
    }

    public IAnQLParserBuilder<TReturn, TItem> RegisterAllProperties()
    {
        foreach (var property in typeof(TItem).GetProperties())
        {
            var anqlPropertyAttribute = property.GetCustomAttribute<AnQLPropertyAttribute>();
            RegisterProperty(anqlPropertyAttribute?.Name, property);
        }

        return this;
    }

    public IAnQLParserBuilder<TReturn, TItem> RegisterTaggedProperties()
    {
        foreach (var property in typeof(TItem).GetProperties())
        {
            var anqlPropertyAttribute = property.GetCustomAttribute<AnQLPropertyAttribute>();
            if (anqlPropertyAttribute != null)
                RegisterProperty(anqlPropertyAttribute.Name, property);
        }

        return this;
    }

    private void RegisterProperty(string? name, PropertyInfo propertyInfo)
    {
        var parameter = Expression.Parameter(typeof(TItem), "x");
        var conv = Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(object));
        var exp = Expression.Lambda<Func<TItem, object>>(conv, parameter);
        if (name != null)
            WithProperty(name, exp);
        else
            WithProperty(exp);
    }

    public IAnQLParserBuilder<TReturn, TItem> WithProperty(Expression<Func<TItem, object>> propertyPath)
    {
        return WithProperty(ExpressionHelper.GetPropertyName(propertyPath), propertyPath);
    }

    public IAnQLParserBuilder<TReturn, TItem> WithProperty(string name, Expression<Func<TItem, object>> propertyPath)
    {
        _resolverMap.AddLazy(name, propertyPath);
        return this;
    }

    public IAnQLParserBuilder<TReturn, TItem> WithProperty(string name, IAnQLPropertyResolver<TReturn> propertyResolver)
    {
        _resolverMap.Add(name, propertyResolver);
        return this;
    }

    public IAnQLParserBuilder<TReturn, TItem> RegisterFactory<TType, TFactory>(TFactory factory)
        where TFactory : IResolverFactory<TItem, TReturn>
    {
        RegisterFactory(typeof(TType), factory);
        return this;
    }
    
    public IAnQLParserBuilder<TReturn, TItem> RegisterFactory(Type type, IResolverFactory<TItem, TReturn> resolverType)
    {
        _resolverMap.RegisterTypeFactory(type, resolverType);
        return this;
    }

    public abstract IAnQLParser<TReturn> Build();
}