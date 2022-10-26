using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using AnQL.Core.Helpers;

namespace AnQL.Core.Resolvers;

public sealed class ResolverMap<TReturn, TItem>
{
    private readonly Dictionary<string, IAnQLPropertyResolver<TReturn>> _lookup = new();
    private readonly Dictionary<Type, IResolverFactory<TItem, TReturn>> _typeFactory = new();
    private readonly Dictionary<string, (Type type, LambdaExpression path)> _lazyProperties = new(); 
    private bool _frozen;

    public void Add(string property, IAnQLPropertyResolver<TReturn> resolver)
    {
        if (_frozen)
            throw new InvalidOperationException("Cannot add properties after map has been frozen");
        
        _lookup.Add(property, resolver);
    }

    public void AddLazy<TValue>(string property, Expression<Func<TItem, TValue>> propertyPath)
    {
        if (_frozen)
            throw new InvalidOperationException("Cannot add properties after map has been frozen");
        
        var type = ((PropertyInfo)((MemberExpression)ExpressionHelper.StripConvert(propertyPath).Body).Member).PropertyType;
        _lazyProperties.Add(property, (type, propertyPath));
    }

    public void RegisterTypeFactory(Type type, IResolverFactory<TItem, TReturn> factory)
    {
        _typeFactory[type] = factory;
    }

    public bool TryGet(string property, [NotNullWhen(true)] out IAnQLPropertyResolver<TReturn>? resolver)
    {
        Freeze();
        return _lookup.TryGetValue(property, out resolver);
    }

    internal void Freeze()
    {
        if (_frozen)
            return;
        
        _frozen = true;
        foreach (var (property, (type, path)) in _lazyProperties)
        {
            if (!_typeFactory.TryGetValue(type, out var factory))
                continue;
            
            var resolver = factory.Build((Expression<Func<TItem, object>>)path);
            _lookup.Add(property, resolver);
        }
    }
}