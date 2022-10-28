using System.Linq.Expressions;
using AnQL.Core;
using AnQL.Expressions.Resolvers;

namespace AnQL.Expressions;

public class ExpressionAnQLParserBuilder<T> : AnQLParserBuilder<Expression<Func<T, bool>>, T>
{
    public ExpressionAnQLParserBuilder(AnQLParserOptions options) : base(options)
    {
    }
    
    public ExpressionAnQLParserBuilder<T> RegisterComparableType<TType>()
        where TType : IComparable<TType>
    {
        return (ExpressionAnQLParserBuilder<T>) RegisterFactory(typeof(TType), new ComparableTypeResolver<T, TType>.Factory());
    }

    public override IAnQLParser<Expression<Func<T, bool>>> Build()
    {
        return new AnQLParser<Expression<Func<T, bool>>>(new AnQLExpressionsVisitor<T>(ResolverMap, Options));
    }
}