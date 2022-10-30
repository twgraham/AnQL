namespace AnQL.Functions.Time;

internal static class Constants
{
    internal static readonly TimeSpan OneTick = TimeSpan.FromTicks(1);
    
    internal static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(string)
    };
}