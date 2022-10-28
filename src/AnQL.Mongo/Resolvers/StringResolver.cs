using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AnQL.Core.Helpers;
using AnQL.Core.Resolvers;
using MongoDB.Driver;

namespace AnQL.Mongo.Resolvers;

public class StringResolver<T> : SimpleResolver<T, string>
{
    private readonly Options _options = new();

    public StringResolver(Expression<Func<T, string>> propertyPath, Action<Options>? configureOptions = null)
        : base(propertyPath)
    {
        configureOptions?.Invoke(_options);
    }

    public override FilterDefinition<T> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        if (_options.RegexMatching && op == QueryOperation.Equal)
            return Builders<T>.Filter.Regex(new ExpressionFieldDefinition<T>(PropertyPath), new Regex(value, _options.RegexOptions));

        return op switch
        {
            QueryOperation.Equal => BuildEqual(value),
            QueryOperation.GreaterThan => BuildGreaterThan(value),
            QueryOperation.LessThan => BuildLessThan(value),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public class Factory : IResolverFactory<T, FilterDefinition<T>>
    {
        public IAnQLPropertyResolver<FilterDefinition<T>> Build(Expression<Func<T, object>> propertyPath)
        {
            var propertyType = ExpressionHelper.GetPropertyPathType(propertyPath);
            if (propertyType != typeof(string))
                throw new ArgumentException("Property should be a string", nameof(propertyPath));

            var stringExpression =
                Expression.Lambda<Func<T, string>>(Expression.Convert(propertyPath.Body, typeof(string)),
                    propertyPath.Parameters);

            return new StringResolver<T>(stringExpression);
        }
    }

    public class Options
    {
        public bool RegexMatching { get; set; } = false;
        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;
    }
}