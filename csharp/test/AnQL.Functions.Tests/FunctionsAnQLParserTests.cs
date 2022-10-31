using System.Collections.Generic;
using System.Linq;
using AnQL.Core;
using AnQL.Functions.Tests.Utilities;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Tests;

public class FunctionsAnQLParserTests
{
    #region Simple property comparisons

    [Fact]
    public void Parse_IntPropertyEquals_ShouldReturnCorrectFunction()
    {
        const string query = "IntProperty: 3";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demo = new DemoClass
        {
            IntProperty = 3
        };

        var queryFunction = anqlParser.Parse(query);

        queryFunction.Should().NotBeNull();
        queryFunction(demo).Should().Be(true);
    }

    [Fact]
    public void Parse_NestedStringProperty_ShouldReturnCorrectFunction()
    {
        const string query = "StringProperty: tes";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithNestedProperties(x => x.NestedDemos,
                ctx => { ctx.WithProperty(x => x.StringProperty, opts => opts.RegexMatching = true); })
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
        queryFunction(demo).Should().Be(true);
    }

    #endregion

    #region AND OR NOT

    [Fact]
    public void ParseWithANDCombinator_SinglePropertyCondition()
    {
        const string query = "IntProperty: > 2 AND IntProperty: < 5";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(2)
            .And.Contain(demoObjects[1..3]);
    }

    [Fact]
    public void ParseWithANDCombinator_MultiPropertyCondition()
    {
        const string query = "IntProperty: > 2 AND StringProperty: foo";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .WithProperty(x => x.StringProperty, conf => conf.RegexMatching = true)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2, StringProperty = "foo" },
            new DemoClass { IntProperty = 3, StringProperty = "foobar" },
            new DemoClass { IntProperty = 4, StringProperty = "barfoo" },
            new DemoClass { IntProperty = 5, StringProperty = "bar" }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(2)
            .And.Contain(demoObjects[1..3]);
    }

    [Fact]
    public void ParseWithORCombinator_SinglePropertyCondition()
    {
        const string query = "IntProperty: > 3 OR IntProperty: 2";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2, StringProperty = "foo" },
            new DemoClass { IntProperty = 3, StringProperty = "foobar" },
            new DemoClass { IntProperty = 4, StringProperty = "barfoo" },
            new DemoClass { IntProperty = 5, StringProperty = "bar" }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(3)
            .And.Contain(new[] { demoObjects[0], demoObjects[2], demoObjects[3] });
    }

    [Fact]
    public void ParseWithORCombinator_MultiPropertyCondition()
    {
        const string query = "IntProperty: > 2 OR StringProperty: bar";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .WithProperty(x => x.StringProperty, conf => conf.RegexMatching = true)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2, StringProperty = "foo" },
            new DemoClass { IntProperty = 3, StringProperty = "foobar" },
            new DemoClass { IntProperty = 4, StringProperty = "barfoo" },
            new DemoClass { IntProperty = 5, StringProperty = "bar" }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(3)
            .And.Contain(demoObjects[1..4]);
    }

    [Fact]
    public void ParseWithNOT_Single()
    {
        const string query = "NOT IntProperty: > 2";

        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2, StringProperty = "foo" },
            new DemoClass { IntProperty = 3, StringProperty = "foobar" },
            new DemoClass { IntProperty = 4, StringProperty = "barfoo" },
            new DemoClass { IntProperty = 5, StringProperty = "bar" }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(1)
            .And.Contain(demoObjects[0]);
    }

    #endregion

    #region Unknown property behaviour

    [Theory]
    [InlineData("NonExistant: < 5 AND IntProperty: > 2")]
    [InlineData("IntProperty: > 2 AND NonExistant: < 5")]
    public void Parse_UnknownProperty_WithIgnore_SimpleAND_ShouldNotBeDeterminant(string query)
    {
        var options = new AnQLParserOptions
        {
            UnknownPropertyBehaviour = UnknownPropertyBehaviour.Ignore
        };

        var anqlParser = new AnQLBuilder(options).ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(3)
            .And.Contain(demoObjects[1..3]);
    }

    [Fact]
    public void Parse_UnknownPropertyNOT_WithIgnore_SimpleAND_ShouldNotBeDeterminant()
    {
        const string query = "NOT NonExistant: < 5 AND IntProperty: > 2";

        var options = new AnQLParserOptions
        {
            UnknownPropertyBehaviour = UnknownPropertyBehaviour.Ignore
        };

        var anqlParser = new AnQLBuilder(options).ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(3)
            .And.Contain(demoObjects[1..3]);
    }

    [Theory]
    [InlineData("NonExistant: < 5 OR IntProperty: > 2")]
    [InlineData("IntProperty: > 2 OR NonExistant: < 5")]
    public void Parse_UnknownProperty_WithIgnore_SimpleOR_ShouldNotBeDeterminant(string query)
    {
        var options = new AnQLParserOptions
        {
            UnknownPropertyBehaviour = UnknownPropertyBehaviour.Ignore
        };

        var anqlParser = new AnQLBuilder(options).ForFunctions<DemoClass>()
            .WithProperty(x => x.IntProperty)
            .Build();

        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };

        var queryFunction = anqlParser.Parse(query);
        queryFunction.Should().NotBeNull();
        demoObjects.Where(queryFunction).Should().HaveCount(3)
            .And.Contain(demoObjects[1..3]);
    }

    #endregion

    #region Standard operation

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Parse_EmptyQuery_ShouldReturnSuccesQuery(string query)
    {
        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>().Build();
        
        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };
        
        var queryFunction = anqlParser.Parse(query);
        demoObjects.Where(queryFunction).Should().Contain(demoObjects);
    }

    #endregion

    #region Register all properties with standard supported types

    [Fact]
    public void Parse_RegisterAllProperties_ShouldWorkForInt()
    {
        var anqlParser = new AnQLBuilder().ForFunctions<DemoClass>().RegisterAllProperties().Build();
        
        var demoObjects = new[]
        {
            new DemoClass { IntProperty = 2 },
            new DemoClass { IntProperty = 3 },
            new DemoClass { IntProperty = 4 },
            new DemoClass { IntProperty = 5 }
        };
        
        var queryFunction = anqlParser.Parse("IntProperty: > 3");
        demoObjects.Where(queryFunction).Should().HaveCount(2)
            .And.Contain(demoObjects[2..4]);
    }

    #endregion
}