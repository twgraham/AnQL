using System.Globalization;
using AnQL.Core.Resolvers;
using Microsoft.Recognizers.Text.DateTime;

namespace AnQL.Functions.Time;

public class DateTimePropertyResolver<T> : IAnQLPropertyResolver<Func<T, bool>>
{
    private static readonly Func<T, bool> AlwaysFalse = _ => false;

    private readonly Func<T, DateTime> _propertyAccessor;
    private readonly Options _options = new();

    public DateTimePropertyResolver(Func<T, DateTime> propertyAccessor, Action<Options>? configureOptions = null)
    {
        _propertyAccessor = propertyAccessor;
        configureOptions?.Invoke(_options);
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        try
        {
            var results = Common.DateTimeModel.Parse(value);

            if (results.Count == 0 || !results[0].TypeName.StartsWith(MergedParserUtil.ParserTypeName))
                return AlwaysFalse;

            var firstResult = results[0];
            var resolutionValues = (IList<Dictionary<string, string>>)firstResult.Resolution["values"];
            var subType = firstResult.TypeName.Split('.').Last();
            
            if (!subType.Contains("date"))
                return AlwaysFalse;

            if (subType.Contains("range"))
            {
                var from = TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(resolutionValues[0]["start"]), _options.TimeZone);
                var to = TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(resolutionValues[0]["end"]), _options.TimeZone);

                if (from > to)
                    (from, to) = (to, from);
                
                return op switch
                {
                    QueryOperation.Equal => BuildWithinRange(from - Common.OneTick, to + Common.OneTick),
                    QueryOperation.GreaterThan => BuildGreaterThan(from),
                    QueryOperation.LessThan => BuildLessThan(to),
                    _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
                };
            }

            var dateValue = TimeZoneInfo.ConvertTime(
                DateTimeOffset.Parse(resolutionValues[0]["value"], CultureInfo.InvariantCulture), _options.TimeZone);

            var (min, max) = ClampDate(dateValue, _options.ClampExactEqualsUnit);
                
            return op switch
            {
                QueryOperation.Equal => BuildWithinRange(min - Common.OneTick, max),
                QueryOperation.GreaterThan => BuildGreaterThan(dateValue),
                QueryOperation.LessThan => BuildLessThan(dateValue),
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
        catch
        {
            return AlwaysFalse;
        }
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