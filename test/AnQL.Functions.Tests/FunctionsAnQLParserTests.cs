using AnQL.Core;
using AnQL.Functions.Tests.Utilities;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Tests;

public class FunctionsAnQLParserTests
{
    [Fact]
    public void Parse_IntPropertyEquals_ShouldReturnCorrectFunction()
    {
        const string query = "IntProperty: 3";
        
        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithValueProperty(x => x.IntProperty)
            .Build();

        var demo = new DemoClass
        {
            IntProperty = 3
        };
        
        var queryFunction = anqlParser.Parse(query);

        queryFunction.Should().NotBeNull();
        queryFunction!(demo).Should().Be(true);
    }
    
    [Fact]
    public void Parse_StringPropertyWithRegex_ShouldReturnCorrectFunction()
    {
        const string query = "StringProperty: tes";
        
        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithValueProperty(x => x.StringProperty, opts =>
            {
                opts.RegexMatching = true;
            })
            .Build();

        var demo = new DemoClass
        {
            StringProperty = "test"
        };
        
        var queryFunction = anqlParser.Parse(query);

        queryFunction.Should().NotBeNull();
        queryFunction!(demo).Should().Be(true);
    }
}