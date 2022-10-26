using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Time.Tests;

public class FunctionsAnQLParserTests
{
    [Fact]
    public void woah()
    {
        var parser = Core.AnQL.Builder.ForFunctions<DemoClass>()
            .AddNaturalTime()
            .RegisterAllProperties()
            .Build();
        
        var demoObjects = new[]
        {
            new DemoClass { DateTimeProperty = new DateTime(2021, 06, 2) },
            new DemoClass { DateTimeProperty = new DateTime(2022, 06, 2) },
            new DemoClass { DateTimeProperty = new DateTime(2022, 07, 2) },
            new DemoClass { DateTimeProperty = new DateTime(2023, 06, 2) }
        };

        var filter = parser.Parse("DateTimeProperty: \"June 2022\"");

        demoObjects.Where(filter).ToList().Should().HaveCount(1).And.Contain(demoObjects[1]);
    }
}