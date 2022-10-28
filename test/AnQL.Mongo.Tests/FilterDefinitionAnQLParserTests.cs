using AnQL.Core;
using AnQL.Mongo.Tests.Utilities;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace AnQL.Mongo.Tests;

public class FilterDefinitionAnQLParserTests
{
    [Fact]
    public void Parse_RegisterAllProperties_ShouldWorkForString()
    {
        var anqlParser = new AnQLBuilder().ForFilterDefinitions<DemoClass>().RegisterAllProperties().Build();

        var filter = anqlParser.Parse("StringProperty: foo");
        var expectedFilter = Builders<DemoClass>.Filter.Eq(x => x.StringProperty, "foo");

        filter.RenderAsJson().Should().Be(expectedFilter.RenderAsJson());
    }
}