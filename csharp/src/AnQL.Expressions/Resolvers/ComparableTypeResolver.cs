using System.Linq.Expressions;
using System.Reflection;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;

namespace AnQL.Expressions.Resolvers;

public class ComparableTypeResolver<T, TValue> : IAnQLPropertyResolver<Expression<Func<T, bool>>> where TValue : IComparable<TValue>
{
    private readonly MethodInfo _compareToMethodInfo = typeof(IComparable<TValue>).GetMethod(nameof(IComparable<TValue>.CompareTo), new [] { typeof(TValue) });
    private readonly Expression<Func<T, TValue>> _propertyPath;
    private readonly Options _options = new();

    public ComparableTypeResolver(Expression<Func<T, TValue>> propertyPath, Action<Options>? configureOptions = null)
    {
        _propertyPath = propertyPath;
        configureOptions?.Invoke(_options);
    }

    public Expression<Func<T, bool>> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        var converter = _options.ValueConverter ?? DefaultConverter;
        var convertedValue = converter(value, valueType);
        
        return op switch
        {
            QueryOperation.Equal => BuildEquals(convertedValue),
            QueryOperation.GreaterThan => BuildGreaterThan(convertedValue),
            QueryOperation.LessThan => BuildLessThan(convertedValue),
            _ => throw new ArgumentOutOfRangeException(nameof(op))
        };
    }
    
    private Expression<Func<T, bool>> BuildEquals(TValue value)
    {
        return Expression.Lambda<Func<T, bool>>(CompareExpression(_propertyPath.Body, value, 0), _propertyPath.Parameters);
    }
    
    private Expression<Func<T, bool>> BuildLessThan(TValue value)
    {
        return Expression.Lambda<Func<T, bool>>(CompareExpression(_propertyPath.Body, value, -1), _propertyPath.Parameters);
    }
    
    public Expression<Func<T, bool>> BuildGreaterThan(TValue value)
    {
        return Expression.Lambda<Func<T, bool>>(CompareExpression(_propertyPath.Body, value, 1), _propertyPath.Parameters);
    }
    
    private BinaryExpression CompareExpression(Expression instance, TValue value, int comparerResult)
    {
        return Expression.Equal(
            Expression.Call(instance, _compareToMethodInfo, Expression.Constant(value, typeof(TValue))),
            Expression.Constant(comparerResult)
        );
    }

    private static TValue DefaultConverter(string value, AnQLValueType valueType)
    {
        return (TValue) Convert.ChangeType(value, typeof(TValue));
    }
    
    public class Factory : IResolverFactory<T, Expression<Func<T, bool>>>
    {
        private static BindingFlags _flags = BindingFlags.CreateInstance |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.OptionalParamBinding;
        
        public IAnQLPropertyResolver<Expression<Func<T, bool>>> Build(Expression<Func<T, object>> propertyPath)
        {
            var propertyType = ExpressionHelper.GetPropertyPathType(propertyPath);
            if (!propertyType.IsAssignableTo(typeof(IComparable<>).MakeGenericType(propertyType)))
                throw new ArgumentException("Property does not implement IComparable<>");
            
            var resolverType = typeof(ComparableTypeResolver<,>).MakeGenericType(typeof(T), propertyType);
            var unconvertedPath = ExpressionHelper.StripConvert(propertyPath);

            var resolver = Activator.CreateInstance(resolverType, _flags, null, new object?[] { unconvertedPath }, null)
                           ?? throw new Exception("Unable to create resolver");

            return (IAnQLPropertyResolver<Expression<Func<T, bool>>>)resolver;
        }
    }

    public class Options
    {
        public Func<string, AnQLValueType, TValue>? ValueConverter { get; set; }
    }
}