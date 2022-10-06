using System.Text.RegularExpressions;
using AnQL.Core.Resolvers;

namespace AnQL.Functions.Resolvers;

public class ValueTypeResolver<T, TItem> : IAnQLPropertyResolver<Func<T, bool>>
    where TItem : IComparable<TItem>
{
    private readonly Func<T, TItem> _propertyAccessor;
    private readonly Options _options = new();
    private readonly bool _isStringType = typeof(TItem) == typeof(string); 

    public ValueTypeResolver(Func<T, TItem> propertyAccessor, Action<Options>? configureOptions = null)
    {
        _propertyAccessor = propertyAccessor;
        configureOptions?.Invoke(_options);
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        var converter = _options.ValueConverter ?? DefaultConverter;
        var convertedValue = converter(value, valueType);

        return op switch
        {
            QueryOperation.Equal => BuildEquals(convertedValue),
            QueryOperation.GreaterThan => BuildGreaterThan(convertedValue),
            QueryOperation.LessThan => BuildLessThan(convertedValue),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private Func<T, bool> BuildEquals(TItem value)
    {
        if (_isStringType && _options.RegexMatching)
        {
            return arg => Regex.IsMatch((_propertyAccessor(arg) as string)!, (value as string)!, _options.RegexOptions,
                _options.RegexTimeout);
        }
        
        return arg => _propertyAccessor(arg).CompareTo(value) == 0;
    }
    
    private Func<T, bool> BuildGreaterThan(TItem value)
    {
        return arg => _propertyAccessor(arg).CompareTo(value) == 1;
    }
    
    private Func<T, bool> BuildLessThan(TItem value)
    {
        return arg => _propertyAccessor(arg).CompareTo(value) == -1;
    }

    private TItem DefaultConverter(string queryValue, AnQLValueType valueType)
    {
        return (TItem) Convert.ChangeType(queryValue, typeof(TItem));
    }

    public class Options
    {
        public Func<string, AnQLValueType, TItem>? ValueConverter { get; set; }
        public bool RegexMatching { get; set; } = false;
        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;
        public TimeSpan RegexTimeout { get; set; } = TimeSpan.FromMilliseconds(1);
    }
}