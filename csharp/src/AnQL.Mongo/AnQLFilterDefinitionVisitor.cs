using AnQL.Core;
using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;
using MongoDB.Driver;

namespace AnQL.Mongo;

public class AnQLFilterDefinitionVisitor<T> : AnQLBaseVisitor<T, FilterDefinition<T>>
{
    public override FilterDefinition<T> SuccessQueryResult => Builders<T>.Filter.Empty;
    public override FilterDefinition<T> FailedQueryResult => "{ $expr: false }";

    public AnQLFilterDefinitionVisitor(ResolverMap<FilterDefinition<T>, T> resolverMap, AnQLParserOptions options) : base(resolverMap, options)
    {
    }

    public override FilterDefinition<T> VisitExprAND(FilterDefinition<T> left, FilterDefinition<T> right)
        => left & right;

    public override FilterDefinition<T> VisitExprOR(FilterDefinition<T> left, FilterDefinition<T> right)
        => left | right;

    public override FilterDefinition<T> VisitExprNOT(FilterDefinition<T> childExpression)
        => !childExpression;

    public override FilterDefinition<T> VisitAnyEqual(params FilterDefinition<T>[] childExpressions)
        => Builders<T>.Filter.Or(childExpressions);
}