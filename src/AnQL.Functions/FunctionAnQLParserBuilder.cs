using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Helpers;
using AnQL.Functions.Fluent;
using AnQL.Functions.Resolvers;

namespace AnQL.Functions;

public sealed class FunctionAnQLParserBuilder<T> : AnQLParserBuilder<Func<T, bool>, T>
{
    public FunctionAnQLParserBuilder(AnQLParserOptions options) : base(options)
    {
    }

    public FunctionAnQLParserBuilder<T> WithProperty<TItem>(Expression<Func<T, TItem>> propertyPath,
        Action<ComparableTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return WithValueProperty(propertyName, propertyAccessor, configureOptions);
    }

    public FunctionAnQLParserBuilder<T> WithProperty(Expression<Func<T, string>> propertyPath,
        Action<StringResolver<T>.Options>? configureOptions = null)
    {
        var propertyName = ExpressionHelper.GetPropertyName(propertyPath);
        var propertyAccessor = propertyPath.Compile();
        return (FunctionAnQLParserBuilder<T>) WithProperty(propertyName, new StringResolver<T>(propertyAccessor, configureOptions));
    }

    public FunctionAnQLParserBuilder<T> WithValueProperty<TItem>(string name, Func<T, TItem> propertyAccessor,
        Action<ComparableTypeResolver<T, TItem>.Options>? configureOptions = null)
        where TItem : IComparable<TItem>
    {
        WithProperty(name, new ComparableTypeResolver<T, TItem>(propertyAccessor, configureOptions));
        return this;
    }

    public FunctionAnQLParserBuilder<T> WithNestedProperties<TItem>(Func<T, IEnumerable<TItem>> collectionPath, Action<NestedPropertyContext<TItem>> configureContext)
    {
        var nestedContext = new NestedPropertyContext<TItem>();
        configureContext(nestedContext);
        var nestedResolverMap = nestedContext.Build();
        foreach (var (key, resolver) in nestedResolverMap)
        {
            WithProperty(key, new EnumerableNestedResolver<T,TItem>(collectionPath, resolver));
        }

        return this;
    }

    public FunctionAnQLParserBuilder<T> RegisterComparableType<TType>()
        where TType : IComparable<TType>
    {
        return (FunctionAnQLParserBuilder<T>) RegisterFactory(typeof(TType), new ComparableTypeResolver<T, TType>.Factory());
    }

    public override IAnQLParser<Func<T, bool>> Build()
    {
        return new AnQLParser<Func<T, bool>>(new AnQLFunctionsVisitor<T>(ResolverMap, Options));
    }
}