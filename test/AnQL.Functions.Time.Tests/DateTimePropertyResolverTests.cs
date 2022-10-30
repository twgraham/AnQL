using System;
using System.Linq;
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

    [Fact]
    public void Resolve_GreaterThanDate()
    {
        const string query = "June 2023";
        var dataset = new[]
        {
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 30) },
            new DemoClass { DateTimeProperty = new DateTime(2023, 7, 2) }
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, query, AnQLValueType.String);

        dataset.Where(func).ToList().Should().HaveCount(1)
            .And.Contain(dataset[1]);
    }
    
    [Fact]
    public void Resolve_LessThanDate()
    {
        const string query = "June 2023";
        var dataset = new[]
        {
            new DemoClass { DateTimeProperty = new DateTime(2023, 5, 31) },
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 2) }
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.LessThan, query, AnQLValueType.String);

        dataset.Where(func).ToList().Should().HaveCount(1)
            .And.Contain(dataset[0]);
    }
    
    [Fact]
    public void Resolve_GreaterThanExactDateTime()
    {
        const string query = "11AM on 3rd June 2023";
        var dataset = new[]
        {
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 3, 8, 0, 0).ToUniversalTime() },
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 3, 12, 0, 0).ToUniversalTime() }
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, query, AnQLValueType.String);

        dataset.Where(func).ToList().Should().HaveCount(1)
            .And.Contain(dataset[1]);
    }
    
    [Fact]
    public void Resolve_LessThanExactDateTime()
    {
        const string query = "11AM on 3rd June 2023";
        var dataset = new[]
        {
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 3, 8, 0, 0).ToUniversalTime() },
            new DemoClass { DateTimeProperty = new DateTime(2023, 6, 3, 12, 0, 0).ToUniversalTime() }
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(QueryOperation.LessThan, query, AnQLValueType.String);

        dataset.Where(func).ToList().Should().HaveCount(1)
            .And.Contain(dataset[0]);
    }

    [Theory]
    [InlineData(QueryOperation.Equal)]
    [InlineData(QueryOperation.GreaterThan)]
    [InlineData(QueryOperation.LessThan)]
    public void Resolve_NonDateValue_ShouldReturnFalse(QueryOperation operation)
    {
        const string query = "hello";
        var dataset = new DemoClass
        {
            DateTimeProperty = new DateTime(2022, 5, 1)
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(operation, query, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(QueryOperation.Equal)]
    [InlineData(QueryOperation.GreaterThan)]
    [InlineData(QueryOperation.LessThan)]
    public void Resolve_TimeValue_ShouldReturnFalse(QueryOperation operation)
    {
        const string query = "12PM";
        var dataset = new DemoClass
        {
            DateTimeProperty = new DateTime(2022, 5, 1)
        };
        var sut = new DateTimePropertyResolver<DemoClass>(x => x.DateTimeProperty);
        var func = sut.Resolve(operation, query, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
}