using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;

namespace AnQL.Functions.Resolvers;

public sealed class ComparableTypeResolver<T, TItem> : IAnQLPropertyResolver<Func<T, bool>>
    where TItem : IComparable<TItem>
{
    private readonly Func<T, TItem> _propertyAccessor;
    private readonly Options _options = new();

    private ComparableTypeResolver(Delegate propertyAccessor)
    {
        _propertyAccessor = (Func<T, TItem>)propertyAccessor;
    }

    public ComparableTypeResolver(Func<T, TItem> propertyAccessor, Action<Options>? configureOptions = null)
    {
        _propertyAccessor = propertyAccessor;
        configureOptions?.Invoke(_options);
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        var converter = _options.ValueConverter ?? DefaultConverter;
        var convertedValue = converter(value, valueType);

        return op switch
        {
            QueryOperation.Equal => ComparableHelpers.BuildEquals(_propertyAccessor, convertedValue),
            QueryOperation.GreaterThan => ComparableHelpers.BuildGreaterThan(_propertyAccessor, convertedValue),
            QueryOperation.LessThan => ComparableHelpers.BuildLessThan(_propertyAccessor, convertedValue),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private TItem DefaultConverter(string queryValue, AnQLValueType valueType)
    {
        return (TItem) Convert.ChangeType(queryValue, typeof(TItem));
    }

    public class Factory : IResolverFactory<T, Func<T, bool>>
    {
        private static BindingFlags _flags = BindingFlags.CreateInstance |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.OptionalParamBinding;
        public IAnQLPropertyResolver<Func<T, bool>> Build(Expression<Func<T, object>> propertyPath)
        {
            var type = ExpressionHelper.GetPropertyPathType(propertyPath);
            var accessor = Expression.Lambda(Expression.Convert(propertyPath.Body, type), propertyPath.Parameters)
                .Compile();

            var resolverType = typeof(ComparableTypeResolver<,>).MakeGenericType(typeof(T), type);
            var resolver = Activator.CreateInstance(resolverType, _flags, null, new [] { accessor }, CultureInfo.InvariantCulture);
            
            return (IAnQLPropertyResolver<Func<T, bool>>)resolver;
        }
    }

    public class Options
    {
        public Func<string, AnQLValueType, TItem>? ValueConverter { get; set; }
    }
}
