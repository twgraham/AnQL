using Microsoft.Recognizers.Text.DateTime;

namespace AnQL.Functions.Time;

internal static class Common
{
    public static readonly DateTimeModel DateTimeModel = new DateTimeRecognizer().GetDateTimeModel();
    public static readonly TimeSpan OneTick = TimeSpan.FromTicks(1);
}