using System.Linq.Expressions;
using AnQL.Core;

namespace AnQL.Expressions;

public class ExpressionAnQLParserBuilder<T> : AnQLParserBuilder<Expression<Func<T, bool>>, T>
{
    public ExpressionAnQLParserBuilder(AnQLParserOptions options) : base(options)
    {
    }

    public override IAnQLParser<Expression<Func<T, bool>>> Build()
    {
        return new AnQLParser<Expression<Func<T, bool>>>(new AnQLExpressionsVisitor<T>(ResolverMap, Options));
    }
}