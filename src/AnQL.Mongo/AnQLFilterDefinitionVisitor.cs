using AnQL.Core;
using AnQL.Core.Extensions;
using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;
using MongoDB.Driver;

namespace AnQL.Mongo;

public class AnQLFilterDefinitionVisitor<T> : AnQLBaseVisitor<FilterDefinition<T>>
{
    private readonly ResolverMap<FilterDefinition<T>, T> _resolverMap;
    
    public override FilterDefinition<T> SuccessQueryResult => Builders<T>.Filter.Empty;
    public override FilterDefinition<T> FailedQueryResult => "{ $expr: false }";

    public AnQLFilterDefinitionVisitor(ResolverMap<FilterDefinition<T>, T> resolverMap, AnQLParserOptions options) : base(options)
    {
        _resolverMap = resolverMap;
    }
    
    public override FilterDefinition<T> VisitExprAND(AnQLGrammarParser.ExprANDContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));

        return left & right;
    }
    
    public override FilterDefinition<T> VisitExprOR(AnQLGrammarParser.ExprORContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));

        return left | right;
    }

    public override FilterDefinition<T> VisitNOT(AnQLGrammarParser.NOTContext context)
    {
        var inner = Visit(context.expr());
        return !inner;
    }

    public override FilterDefinition<T> VisitParens(AnQLGrammarParser.ParensContext context)
    {
        return Visit(context.expr());
    }
    
    public override FilterDefinition<T> VisitEqual(AnQLGrammarParser.EqualContext context)
    {
        return BuildFilter(QueryOperation.Equal, context.property_path(), context.value());
    }

    public override FilterDefinition<T> VisitGreaterThan(AnQLGrammarParser.GreaterThanContext context)
    {
        return BuildFilter(QueryOperation.GreaterThan, context.property_path(), context.value());
    }

    public override FilterDefinition<T> VisitLessThan(AnQLGrammarParser.LessThanContext context)
    {
        return BuildFilter(QueryOperation.LessThan, context.property_path(), context.value());
    }

    private FilterDefinition<T> BuildFilter(QueryOperation operation, AnQLGrammarParser.Property_pathContext propertyPathContext,
        AnQLGrammarParser.ValueContext valueContext)
    {
        if (!_resolverMap.TryGet(propertyPathContext.GetText(), out var resolver))
            return HandleUnknownProperty(propertyPathContext);

        var (value, type) = valueContext.GetValueAndAnQLType();

        return resolver.Resolve(operation, value, type);
    }
}