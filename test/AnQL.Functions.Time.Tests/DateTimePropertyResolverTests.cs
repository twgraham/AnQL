using System;
using AnQL.Core.Resolvers;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Time.Tests;

public class DateTimePropertyResolverTests
{
    [Theory]
    [InlineData(2012, "2012")]
    [InlineData(1860, "1860")]
    public void Resolve_YearEqualsValid_ShouldReturnTrue(int year, string queryValue)
    {
        var dataset = new DemoClass
        {
            DateTimeProperty = new DateTime(year, 1, 1)
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
    
    [Theory]
    [InlineData(13)]
    [InlineData(1)]
    [InlineData(30)]
    public void Resolve_MonthEqualsValid_ShouldReturnTrue(int day)
    {
        const string query = "June 2022";
        var dataset = new DemoClass
        {
            DateTimeProperty = new DateTime(2022, 6, day)
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.Equal, query, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
    
    [Fact]
    public void Resolve_MonthEqualsReverseRange_ShouldReturnTrue()
    {
        const string query = "Between June 2023 and March 2022";
        var dataset = new DemoClass
        {
            DateTimeProperty = new DateTime(2022, 5, 1)
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.Equal, query, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
}