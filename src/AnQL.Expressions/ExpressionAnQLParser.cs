using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Core.Resolvers;

namespace AnQL.Expressions;

public class ExpressionAnQLParser<T> : IAnQLParser<Expression<Func<T, bool>>?>
{
    private readonly AnQLExpressionsVisitor<T> _visitor;
    public ExpressionAnQLParser(Dictionary<string, IAnQLPropertyResolver<Expression<Func<T, bool>>>> resolverMap)
    {
        _visitor = new AnQLExpressionsVisitor<T>(resolverMap);
    }

    public Expression<Func<T, bool>>? Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        var anqlParser = Core.AnQL.BuildParser(input);

        return _visitor.Visit(anqlParser.query());
    }
}