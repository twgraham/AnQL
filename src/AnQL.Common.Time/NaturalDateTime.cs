using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Recognizers.Text.DateTime;

namespace AnQL.Common.Time;

public class NaturalDateTime
{
    private static readonly DateTimeModel DateTimeModel = new DateTimeRecognizer().GetDateTimeModel();
    
    public static bool TryConvert(string value, TimeZoneInfo timeZoneInfo, [NotNullWhen(true)] out DateTimeOffset? from, out DateTimeOffset? to)
    {
        from = to = null;
        try
        {
            var results = DateTimeModel.Parse(value);

            if (results.Count == 0 || !results[0].TypeName.StartsWith(MergedParserUtil.ParserTypeName))
                return false;

            var firstResult = results[0];
            var resolutionValues = (IList<Dictionary<string, string>>)firstResult.Resolution["values"];
            var subType = firstResult.TypeName.Split('.').Last();
            
            if (!subType.Contains("date"))
                return false;

            if (subType.Contains("range"))
            {
                from = TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(resolutionValues[0]["start"]), timeZoneInfo);
                to = TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(resolutionValues[0]["end"]), timeZoneInfo);

                if (from > to)
                    (from, to) = (to, from);
            }
            else
            {
                from = TimeZoneInfo.ConvertTime(
                    DateTimeOffset.Parse(resolutionValues[0]["value"], CultureInfo.InvariantCulture), timeZoneInfo);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static (DateTimeOffset Min, DateTimeOffset Max) ClampDate(DateTimeOffset value, TimeUnit timeUnit)
    {
        return timeUnit switch
        {
            TimeUnit.Second => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second,
                    value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second,
                    value.Offset).AddSeconds(1)
            ),
            TimeUnit.Minute => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Offset).AddMinutes(1)
            ),
            TimeUnit.Hour => (
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Offset).AddHours(1)
            ),
            TimeUnit.Day => (
                new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset).AddDays(1)
            ),
            TimeUnit.Month => (
                new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset).AddMonths(1)
            ),
            TimeUnit.Year => (
                new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset),
                new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset).AddYears(1)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
        };
    }
}