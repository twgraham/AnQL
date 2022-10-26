namespace AnQL.Functions.Resolvers;

public static class ComparableHelpers
{
    public static Func<T, bool> BuildEquals<T, TValue>(Func<T, TValue> propertyAccessor, TValue value)
        where TValue : IComparable<TValue>
    {
        return arg => propertyAccessor(arg).CompareTo(value) == 0;
    }
    
    public static Func<T, bool> BuildLessThan<T, TValue>(Func<T, TValue> propertyAccessor, TValue value)
        where TValue : IComparable<TValue>
    {
        return arg => propertyAccessor(arg).CompareTo(value) == -1;
    }
    
    public static Func<T, bool> BuildGreaterThan<T, TValue>(Func<T, TValue> propertyAccessor, TValue value)
        where TValue : IComparable<TValue>
    {
        return arg => propertyAccessor(arg).CompareTo(value) == 1;
    }
}