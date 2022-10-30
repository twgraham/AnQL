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

        if (to != null)
        {
            return op switch
            {
                QueryOperation.Equal => BuildWithinRange(from.Value - Constants.OneTick, to.Value + Constants.OneTick),
                QueryOperation.GreaterThan => BuildGreaterThan(from.Value),
                QueryOperation.LessThan => BuildLessThan(to.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
        
        var (min, max) = ClampDate(from.Value, _options.ClampExactEqualsUnit);

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

    private static (DateTimeOffset Min, DateTimeOffset Max) ClampDate(DateTimeOffset value, TimeUnit timeUnit)
    {
        return timeUnit switch
        {
            TimeUnit.Second => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second,
                    value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second + 1,
                    value.Offset)),
            TimeUnit.Minute => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute + 1, 0, value.Offset)),
            TimeUnit.Hour => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour + 1, 0, 0, value.Offset)
            ),
            TimeUnit.Day => (
                new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day + 1, 0, 0, 0, value.Offset)
            ),
            TimeUnit.Month => (
                new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month + 1, 1, 0, 0, 0, value.Offset)
            ),
            TimeUnit.Year => (
                new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year + 1, 1, 1, 0, 0, 0, value.Offset)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
        };
    }
    
    private static Func<T, DateTimeOffset> CachedAccessor(Func<T, DateTimeOffset> accessor)
    {
        DateTimeOffset? cache = null;
        return arg => cache ??= accessor(arg);
    }

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
    
    public enum TimeUnit
    {
        Second,
        Minute,
        Hour,
        Day,
        Month,
        Year
    }

    public class Options
    {
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
        public TimeUnit ClampExactEqualsUnit { get; set; } = TimeUnit.Day;
    }
}