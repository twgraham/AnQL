using System.Collections.Generic;
using AnQL.Core.Resolvers;
using AnQL.Functions.Resolvers;
using AnQL.Functions.Tests.Utilities;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Tests.Resolvers;

public class EnumerableNestedResolverTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("def")]
    [InlineData("ghi")]
    public void Resolve_ValidEnumerableAndNestedResolver_AndMatchingQuery_ShouldReturnTrue(string queryValue)
    {
        var dataset = new DemoClass
        {
            NestedDemos = new List<NestedDemoClass>
            {
                new() { StringProperty = "abc" },
                new() { StringProperty = "def" },
                new() { StringProperty = "ghi" }
            }
        };
        var sut = new EnumerableNestedResolver<DemoClass, NestedDemoClass>(x => x.NestedDemos,
            new ComparableTypeResolver<NestedDemoClass, string>(x => x.StringProperty));
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData(" ")]
    public void Resolve_ValidEnumerableAndNestedResolver_AndNotMatchingQuery_ShouldReturnTrue(string queryValue)
    {
        var dataset = new DemoClass
        {
            NestedDemos = new List<NestedDemoClass>
            {
                new() { StringProperty = "abc" },
                new() { StringProperty = "def" },
                new() { StringProperty = "ghi" }
            }
        };
        var sut = new EnumerableNestedResolver<DemoClass, NestedDemoClass>(x => x.NestedDemos,
            new ComparableTypeResolver<NestedDemoClass, string>(x => x.StringProperty));
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
}