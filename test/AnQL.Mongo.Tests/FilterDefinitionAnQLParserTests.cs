using AnQL.Core;
using AnQL.Mongo.Tests.Utilities;
using MongoDB.Driver;
using Xunit;

namespace AnQL.Mongo.Tests;

public class FilterDefinitionAnQLParserTests
{
    private readonly IAnQLParser<FilterDefinition<DemoClass>> _parser;

    public FilterDefinitionAnQLParserTests()
    {
        _parser = new AnQLBuilder().ForFilterDefinitions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .WithProperty(x => x.StringProperty)
            .Build();
    }

    [Fact]
    public void Parse_IntPropertyEquals_ShouldReturnFilter()
    {
        const string query = "IntProperty: 3";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty == 3);
    }
    
    [Fact]
    public void Parse_IntPropertyGreaterThan_ShouldReturnFilter()
    {
        const string query = "IntProperty: > 3";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty > 3);
    }
    
    [Fact]
    public void Parse_IntPropertyLessThan_ShouldReturnFilter()
    {
        const string query = "IntProperty: < 3";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty < 3);
    }
    
    [Fact]
    public void Parse_ANDExpression_ShouldReturnFilter()
    {
        const string query = "IntProperty: 3 AND StringProperty: foo";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty == 3 && x.StringProperty == "foo");
    }
    
    [Fact]
    public void Parse_ORExpression_ShouldReturnFilter()
    {
        const string query = "IntProperty: 3 OR StringProperty: foo";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty == 3 || x.StringProperty == "foo");
    }
    
    [Fact]
    public void Parse_NotExpression_ShouldReturnFilter()
    {
        const string query = "NOT IntProperty: 3";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.IntProperty != 3);
    }
    
    [Fact]
    public void Parse_ParensExpression_ShouldReturnFilter()
    {
        const string query = "StringProperty: bar OR (IntProperty: 3 AND StringProperty: foo)";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.StringProperty == "bar" || (x.IntProperty == 3 && x.StringProperty == "foo"));
    }
    
    [Fact]
    public void Parse_AnyEqualExpression_ShouldReturnFilter()
    {
        const string query = "StringProperty: foo,bar,baz";
        var filter = _parser.Parse(query);
        filter.Should().BeEquivalentTo(x => x.StringProperty == "foo" || x.StringProperty == "bar" || x.StringProperty == "baz");
    }
    
    [Fact]
    public void Parse_RegisterAllProperties_ShouldWorkForString()
    {
        var anqlParser = new AnQLBuilder().ForFilterDefinitions<DemoClass>().RegisterAllProperties().Build();

        var filter = anqlParser.Parse("StringProperty: foo");
        var expectedFilter = Builders<DemoClass>.Filter.Eq(x => x.StringProperty, "foo");

        filter.Should().BeEquivalentTo(expectedFilter);
    }

    [Fact]
    public void Parse_UnknownPropertyWithSuccessDefault_ShouldRenderEmpty()
    {
        const string query = "UnknownProperty: 12";
        var filter = _parser.Parse(query);

        filter.Should().BeEquivalentTo(FilterDefinition<DemoClass>.Empty);
    }
    
    [Fact]
    public void Parse_UnknownPropertyWithFailDefault_ShouldRenderFalse()
    {
        const string query = "UnknownProperty: 12";
        var failUnknownParser = new AnQLBuilder(new AnQLParserOptions
        {
            UnknownPropertyBehaviour = UnknownPropertyBehaviour.Fail
        }).ForFilterDefinitions<DemoClass>().Build();
        var filter = failUnknownParser.Parse(query);

        filter.Should().BeEquivalentTo("{ $expr: false }");
    }
}