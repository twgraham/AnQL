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

                return true;
            }

            from = TimeZoneInfo.ConvertTime(
                DateTimeOffset.Parse(resolutionValues[0]["value"], CultureInfo.InvariantCulture), timeZoneInfo);

            return true;
        }
        catch
        {
            return false;
        }
    }
}