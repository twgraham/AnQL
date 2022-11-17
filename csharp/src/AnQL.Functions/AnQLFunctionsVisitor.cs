using AnQL.Core;
using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;

namespace AnQL.Functions;

public class AnQLFunctionsVisitor<T> : AnQLBaseVisitor<T, Func<T, bool>>
{
    private static readonly Func<T, bool> AlwaysTrue = _ => true;
    private static readonly Func<T, bool> AlwaysFalse = _ => false;
    
    public override Func<T, bool> SuccessQueryResult => AlwaysTrue;
    public override Func<T, bool> FailedQueryResult => AlwaysFalse;

    public AnQLFunctionsVisitor(ResolverMap<Func<T, bool>, T> resolverMap, AnQLParserOptions options) : base(resolverMap, options)
    {
    }
    
    public override Func<T, bool> VisitAnd(Func<T, bool> left, Func<T, bool> right) =>
        value => left(value) && right(value);

    public override Func<T, bool> VisitOr(Func<T, bool> left, Func<T, bool> right) =>
        value => left(value) || right(value);

    public override Func<T, bool> VisitNot(Func<T, bool> childExpression) =>
        value => !childExpression(value);
}