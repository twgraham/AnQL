using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Resolvers;

namespace AnQL.Expressions;

internal class ExpressionAnQLParserBuilder<T> : IAnQLParserBuilder<Expression<Func<T, bool>>?, T>
{
    private readonly AnQLParserOptions _options;
    private readonly Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> _resolverMap = new();

    public ExpressionAnQLParserBuilder(AnQLParserOptions options)
    {
        _options = options;
    }

    public IAnQLParserBuilder<Expression<Func<T, bool>>?, T> WithProperty<TValue>(string name, Expression<Func<T, TValue>> propertyPath)
    {
        throw new NotImplementedException();
    }

    public IAnQLParserBuilder<Expression<Func<T, bool>>?, T> WithProperty<TValue>(Expression<Func<T, TValue>> propertyPath)
    {
        throw new NotImplementedException();
    }

    public IAnQLParserBuilder<Expression<Func<T, bool>>?, T> WithProperty(string name, IAnQLPropertyResolver<Func<T, bool>?> propertyResolver)
    {
        throw new NotImplementedException();
    }

    public IAnQLParser<Expression<Func<T, bool>>?> Build()
    {
        return new ExpressionAnQLParser<T>(_resolverMap);
    }
}