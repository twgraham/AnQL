using System.Linq.Expressions;
using System.Reflection;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;
using MongoDB.Driver;

namespace AnQL.Mongo.Resolvers;

public class SimpleResolver<T, TValue> : IAnQLPropertyResolver<FilterDefinition<T>>
{
    private readonly Options _options = new();

    protected Expression<Func<T, TValue>> PropertyPath { get; }
    
    public SimpleResolver(Expression<Func<T, TValue>> propertyPath, Action<Options>? configureOptions = null)
    {
        PropertyPath = propertyPath;
        configureOptions?.Invoke(_options);
    }

    public virtual FilterDefinition<T> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        var converter = _options.ValueConverter ?? DefaultConverter;
        var convertedValue = converter(value, valueType);
        
        return op switch
        {
            QueryOperation.Equal => BuildEqual(convertedValue),
            QueryOperation.GreaterThan => BuildGreaterThan(convertedValue),
            QueryOperation.LessThan => BuildLessThan(convertedValue),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
    
    protected FilterDefinition<T> BuildEqual(TValue value)
        => Builders<T>.Filter.Eq(PropertyPath, value);

    protected FilterDefinition<T> BuildGreaterThan(TValue value)
        => Builders<T>.Filter.Gt(PropertyPath, value);

    protected FilterDefinition<T> BuildLessThan(TValue value)
        => Builders<T>.Filter.Lt(PropertyPath, value);
    
    private TValue DefaultConverter(string queryValue, AnQLValueType valueType)
    {
        return (TValue) Convert.ChangeType(queryValue, typeof(TValue));
    }

    public class Factory : IResolverFactory<T, FilterDefinition<T>>
    {
        private static BindingFlags _flags = BindingFlags.CreateInstance |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.OptionalParamBinding;
        
        public IAnQLPropertyResolver<FilterDefinition<T>> Build(Expression<Func<T, object>> propertyPath)
        {
            var propertyType = ExpressionHelper.GetPropertyPathType(propertyPath);
            var resolverType = typeof(SimpleResolver<,>).MakeGenericType(typeof(T), propertyType);
            var unconvertedPath = ExpressionHelper.StripConvert(propertyPath);

            var resolver = Activator.CreateInstance(resolverType, _flags, null, new object?[] { unconvertedPath }, null)
                ?? throw new Exception("Unable to create resolver");

            return (IAnQLPropertyResolver<FilterDefinition<T>>)resolver;
        }
    }

    public class Options
    {
        public Func<string, AnQLValueType, TValue>? ValueConverter { get; set; }
    }
}