using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Resolvers;

namespace AnQL.Expressions;

internal class ExpressionAnQLParserBuilder<T> : IAnQLParserBuilder<Expression<Func<T, bool>>?>
{
    private readonly AnQLParserOptions _options;
    private readonly Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> _resolverMap = new();

    public ExpressionAnQLParserBuilder(AnQLParserOptions options)
    {
        _options = options;
    }

    public IAnQLParser<Expression<Func<T, bool>>?> Build()
    {
        return new ExpressionAnQLParser<T>(_resolverMap);
    }
}