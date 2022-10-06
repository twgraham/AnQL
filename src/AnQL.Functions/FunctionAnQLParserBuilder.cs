using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;
using AnQL.Functions.Resolvers;

namespace AnQL.Functions;

public class FunctionAnQLParserBuilder<T> : IAnQLParserBuilder<Func<T, bool>?, T>
{
    private readonly AnQLParserOptions _options;
    private readonly Dictionary<string, IAnQLPropertyResolver<Func<T, bool>?>> _resolverMap = new();

    public FunctionAnQLParserBuilder(AnQLParserOptions options)
    {
        _options = options;
    }

    public IAnQLParserBuilder<Func<T, bool>?, T> WithValueProperty<TItem>(Expression<Func<T, TItem>> propertyPath,
        Action<ValueTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return WithValueProperty(propertyName, propertyAccessor, configureOptions);
    }

    public IAnQLParserBuilder<Func<T, bool>, T> WithValueProperty<TItem>(string name, Func<T, TItem> propertyAccessor,
        Action<ValueTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        _resolverMap.Add(name, new ValueTypeResolver<T, TItem>(propertyAccessor, configureOptions));
        return this;
    }

    public IAnQLParserBuilder<Func<T, bool>?, T> WithProperty<TItem>(string name, IAnQLPropertyResolver<Func<T, bool>?> propertyResolver)
    {
        _resolverMap.Add(name, propertyResolver);
        return this;
    }

    public IAnQLParserBuilder<Func<T, bool>?, T> WithProperty<TValue>(string name, Expression<Func<T, TValue>> propertyPath)
        where TValue : IComparable<TValue>
    {
        return WithValueProperty(name, propertyPath.Compile());
    }

    public IAnQLParserBuilder<Func<T, bool>?, T> WithProperty<TValue>(Expression<Func<T, TValue>> propertyPath)
    {
        throw new NotImplementedException();
    }

    public IAnQLParserBuilder<Func<T, bool>?, T> WithProperty(string name, IAnQLPropertyResolver<Func<T, bool>?> propertyResolver)
    {
        throw new NotImplementedException();
    }

    public IAnQLParser<Func<T, bool>?> Build()
    {
        return new FunctionAnQLParser<T>(_resolverMap);
    }
}