using System.Linq.Expressions;

namespace AnQL.Expressions.Helpers;

internal class ExpressionReplacer : ExpressionVisitor
{
    private readonly Expression Source, Target;

    public ExpressionReplacer(Expression source, Expression target)
    {
        Source = source;
        Target = target;
    }

    public override Expression Visit(Expression node)
    {
        return node == Source ? Target : base.Visit(node);
    }
}