using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AnQL.Core.Resolvers;

namespace AnQL.Functions.Resolvers;

public class StringResolver<T> : IAnQLPropertyResolver<Func<T, bool>>
{
    private readonly Func<T, string> _propertyAccessor;
    private readonly Options _options = new();
    
    public StringResolver(Func<T, string> propertyAccessor, Action<Options>? configureOptions = null)
    {
        _propertyAccessor = propertyAccessor;
        configureOptions?.Invoke(_options);
    }

    public Func<T, bool> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        return op switch
        {
            QueryOperation.Equal => BuildEquals(value),
            QueryOperation.GreaterThan => ComparableHelpers.BuildGreaterThan(_propertyAccessor, value),
            QueryOperation.LessThan => ComparableHelpers.BuildLessThan(_propertyAccessor, value),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private Func<T, bool> BuildEquals(string value)
    {
        if (_options.RegexMatching)
        {
            return arg => Regex.IsMatch(_propertyAccessor(arg), value, _options.RegexOptions,
                _options.RegexTimeout);
        }

        return ComparableHelpers.BuildEquals(_propertyAccessor, value);
    }

    public class Factory : IResolverFactory<T, Func<T, bool>>
    {
        public IAnQLPropertyResolver<Func<T, bool>> Build(Expression<Func<T, object>> propertyPath)
        {
            var accessor = Expression.Lambda<Func<T, string>>(Expression.Convert(propertyPath.Body, typeof(string)), propertyPath.Parameters).Compile();
            return new StringResolver<T>(accessor);
        }
    }

    public class Options
    {
        public bool RegexMatching { get; set; } = false;
        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;
        public TimeSpan RegexTimeout { get; set; } = TimeSpan.FromMilliseconds(1);
    }
}