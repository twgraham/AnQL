using AnQL.Core.Resolvers;
using AnQL.Functions.Resolvers;
using AnQL.Functions.Tests.Utilities;
using FluentAssertions;
using Xunit;

namespace AnQL.Functions.Tests.Resolvers;

public class ComparableTypeResolverTests
{
    #region Integer values
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(0, "0")]
    [InlineData(-144, "-144")]
    public void Resolve_IntEqualsValid_ShouldReturnTrue(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeTrue();
    }
    
    [Theory]
    [InlineData(4, "3")]
    [InlineData(3, "4")]
    [InlineData(1, "0")]
    [InlineData(-144, "144")]
    public void Resolve_IntEqualsInvalid_ShouldReturnFalse(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(4, "3")]
    [InlineData(1, "0")]
    [InlineData(144, "-144")]
    public void Resolve_IntGreaterThanValid_ShouldReturnTrue(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeTrue();
    }

    [Theory]
    [InlineData(3, "4")]
    [InlineData(0, "1")]
    [InlineData(-144, "144")]
    [InlineData(1, "1")]
    public void Resolve_IntGreaterThanInvalid_ShouldReturnFalse(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(3, "4")]
    [InlineData(0, "1")]
    [InlineData(-144, "144")]
    public void Resolve_IntLessThanValid_ShouldReturnTrue(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeTrue();
    }

    [Theory]
    [InlineData(4, "3")]
    [InlineData(1, "0")]
    [InlineData(144, "-144")]
    [InlineData(1, "1")]
    public void Resolve_IntLessThanInvalid_ShouldReturnFalse(int datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            IntProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, int>(x => x.IntProperty);
        var func = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.Number);
        
        func(dataset).Should().BeFalse();
    }

    #endregion
    
    #region String values
    
    [Theory]
    [InlineData("foo", "foo")]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    public void Resolve_StringEqualsValid_ShouldReturnTrue(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
    
    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("", "bar")]
    [InlineData(" ", "")]
    public void Resolve_StringEqualsInvalid_ShouldReturnFalse(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
    
    [Theory]
    [InlineData("b", "a")]
    [InlineData("zoo", "zebra")]
    [InlineData(" ", "")]
    public void Resolve_StringGreaterThanValid_ShouldReturnTrue(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }

    [Theory]
    [InlineData("a", "b")]
    [InlineData("zebra", "zoo")]
    [InlineData("", " ")]
    [InlineData("foo", "foo")]
    public void Resolve_StringGreaterThanInvalid_ShouldReturnFalse(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
    
    [Theory]
    [InlineData("a", "b")]
    [InlineData("zebra", "zoo")]
    [InlineData("", " ")]
    public void Resolve_StringLessThanValid_ShouldReturnTrue(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeTrue();
    }
    
    [Theory]
    [InlineData("b", "a")]
    [InlineData("zoo", "zebra")]
    [InlineData(" ", "")]
    [InlineData("foo", "foo")]
    public void Resolve_StringLessThanInvalid_ShouldReturnFalse(string datasetValue, string queryValue)
    {
        var dataset = new DemoClass
        {
            StringProperty = datasetValue
        };
        var sut = new ComparableTypeResolver<DemoClass, string>(x => x.StringProperty);
        var func = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.String);
        
        func(dataset).Should().BeFalse();
    }
    
    #endregion
}