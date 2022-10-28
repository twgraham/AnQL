using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AnQL.Mongo.Tests.Utilities;

public class FilterDefinitionAssertions<T>
    : ReferenceTypeAssertions<FilterDefinition<T>, FilterDefinitionAssertions<T>>
{
    public FilterDefinitionAssertions(FilterDefinition<T> instance) : base(instance)
    {
    }

    protected override string Identifier => "filter definition";

    public AndConstraint<FilterDefinitionAssertions<T>> BeEquivalentTo(
        Expression<Func<T, bool>> expected, string because = "", params object[] becauseArgs)
    {
        return BeEquivalentTo((FilterDefinition<T>)expected, because, becauseArgs);
    }
    
    public AndConstraint<FilterDefinitionAssertions<T>> BeEquivalentTo(
        FilterDefinition<T> expected, string because = "", params object[] becauseArgs)
    {
        var renderedSubject = Subject.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry)
            .ToJson();
        var renderedExpected = expected.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry).ToJson();

        renderedSubject.Should().Be(renderedExpected, because, becauseArgs);

        return new AndConstraint<FilterDefinitionAssertions<T>>(this);
    }
}