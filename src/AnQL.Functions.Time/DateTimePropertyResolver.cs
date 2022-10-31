using System.Linq.Expressions;
using System.Reflection;
using AnQL.Common.Time;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;

namespace AnQL.Functions.Time;

public class DateTimePropertyResolver<T> : IAnQLPropertyResolver<Func<T, bool>>
{
    private static readonly Func<T, bool> AlwaysFalse = _ => false;

    private readonly Func<T, DateTimeOffset> _propertyAccessor;
    private readonly Options _options = new();

    public DateTimePropertyResolver(Func<T, DateTimeOffset> propertyAccessor, Action<Options>? configureOptions = null)
    {
        _propertyAccessor = propertyAccessor;
        configureOptions?.Invoke(_options);
    }
    
    public DateTimePropertyResolver(Func<T, DateTime> propertyAccessor, Action<Options>? configureOptions = null)
        : this(ToDateTimeOffsetFunc(propertyAccessor), configureOptions)
    {
    }
    
    public DateTimePropertyResolver(Func<T, string> propertyAccessor, Action<Options>? configureOptions = null)
        : this(ToDateTimeOffsetFunc(propertyAccessor), configureOptions)
    {
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        if (!NaturalDateTime.TryConvert(value, _options.TimeZone, out var from, out var to))
            return AlwaysFalse;

        if (to.HasValue)
        {
            return op switch
            {
                QueryOperation.Equal => BuildWithinRange(from.Value - Constants.OneTick, to.Value + Constants.OneTick),
                QueryOperation.GreaterThan => BuildGreaterThan(to.Value),
                QueryOperation.LessThan => BuildLessThan(from.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
        
        var (min, max) = NaturalDateTime.ClampDate(from.Value, _options.ClampExactEqualsUnit);

        return op switch
        {
            QueryOperation.Equal => BuildWithinRange(min - Constants.OneTick, max),
            QueryOperation.GreaterThan => BuildGreaterThan(from.Value),
            QueryOperation.LessThan => BuildLessThan(from.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private Func<T, bool> BuildWithinRange(DateTimeOffset from, DateTimeOffset to)
    {
        var greaterThan = BuildGreaterThan(from);
        var lessThan = BuildLessThan(to);

        return arg => greaterThan(arg) && lessThan(arg);
    }

    private Func<T, bool> BuildGreaterThan(DateTimeOffset value) => arg => _propertyAccessor(arg) > value;

    private Func<T, bool> BuildLessThan(DateTimeOffset value) => arg => _propertyAccessor(arg) < value;

    private static Func<T, DateTimeOffset> ToDateTimeOffsetFunc(Func<T, DateTime> func)
        => arg => new DateTimeOffset(func(arg));
    
    private static Func<T, DateTimeOffset> ToDateTimeOffsetFunc(Func<T, string> func)
        => arg => DateTimeOffset.Parse(func(arg));

    public class Factory : IResolverFactory<T, Func<T, bool>>
    {
        public IAnQLPropertyResolver<Func<T, bool>> Build(Expression<Func<T, object>> propertyPath)
        {
            var type = ExpressionHelper.GetPropertyPathType(propertyPath);

            if (!Constants.SupportedTypes.Contains(type))
                throw new ArgumentException(
                    $"Property of type {type} cannot be used by {nameof(DateTimePropertyResolver<T>)}. Supported types: {string.Join(", ", Constants.SupportedTypes)}");

            var path = Expression.Lambda(ExpressionHelper.StripConvert(propertyPath).Body, propertyPath.Parameters);
            var resolver = Activator.CreateInstance(typeof(DateTimePropertyResolver<T>), BindingFlags.CreateInstance |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.OptionalParamBinding, null, new object?[] { path.Compile(), null }, null);
            return (DateTimePropertyResolver<T>)resolver;
        }
    }

    public class Options
    {
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
        public TimeUnit ClampExactEqualsUnit { get; set; } = TimeUnit.Day;
    }
}