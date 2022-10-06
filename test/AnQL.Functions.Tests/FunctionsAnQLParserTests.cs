using System.Collections.Generic;
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
    public void Parse_NestedStringProperty_ShouldReturnCorrectFunction()
    {
        const string query = "StringProperty: tes";
        
        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithNestedProperties(x => x.NestedDemos, ctx =>
            {
                ctx.WithValueProperty(x => x.StringProperty, opts => opts.RegexMatching = true);
            })
            .Build();

        var demo = new DemoClass
        {
            NestedDemos = new List<NestedDemoClass>
            {
                new() { StringProperty = "test" }
            }
        };
        
        var queryFunction = anqlParser.Parse(query);

        queryFunction.Should().NotBeNull();
        queryFunction!(demo).Should().Be(true);
    }
}