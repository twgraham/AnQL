using AnQL.Core.Resolvers;
using AnQL.Mongo.Resolvers;
using AnQL.Mongo.Tests.Utilities;
using MongoDB.Driver;
using Xunit;

namespace AnQL.Mongo.Tests.Resolvers;

public class SimpleResolverTests
{
    #region Integer values
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(0, "0")]
    [InlineData(-144, "-144")]
    public void Resolve_IntEquals_ShouldRender(int datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, int>(x => x.IntProperty);
        var filter = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.Number);

        filter.Should().BeEquivalentTo(x => x.IntProperty == datasetValue);
    }

    [Theory]
    [InlineData(3, "3")]
    [InlineData(0, "0")]
    [InlineData(-144, "-144")]
    public void Resolve_IntGreaterThan_ShouldRender(int datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, int>(x => x.IntProperty);
        var filter = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.Number);

        filter.Should().BeEquivalentTo(x => x.IntProperty > datasetValue);
    }
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(0, "0")]
    [InlineData(-144, "-144")]
    public void Resolve_IntLessThan_ShouldRender(int datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, int>(x => x.IntProperty);
        var filter = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.Number);

        filter.Should().BeEquivalentTo(x => x.IntProperty < datasetValue);
    }

    #endregion
    
    #region String values
    
    [Theory]
    [InlineData("b")]
    [InlineData("zoo")]
    [InlineData(" ")]
    public void Resolve_StringEquals_ShouldRender(string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, string>(x => x.StringProperty);
        var filter = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);

        filter.Should().BeEquivalentTo(x => x.StringProperty == queryValue);
    }

    [Theory]
    [InlineData("b")]
    [InlineData("zoo")]
    [InlineData(" ")]
    public void Resolve_StringGreaterThan_ShouldRender(string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, string>(x => x.StringProperty);
        var filter = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.String);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Gt(x => x.StringProperty, queryValue));
    }

    [Theory]
    [InlineData("b")]
    [InlineData("zoo")]
    [InlineData(" ")]
    public void Resolve_StringLessThan_ShouldRender(string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, string>(x => x.StringProperty);
        var filter = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.String);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Lt(x => x.StringProperty, queryValue));
    }
    
    #endregion
    
    #region Boolean values
    
    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Resolve_BoolEquals_ShouldRender(bool datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, bool>(x => x.BoolProperty);
        var filter = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.Bool);

        filter.Should().BeEquivalentTo(x => x.BoolProperty == datasetValue);
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Resolve_BoolGreaterThan_ShouldRender(bool datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, bool>(x => x.BoolProperty);
        var filter = sut.Resolve(QueryOperation.GreaterThan, queryValue, AnQLValueType.Bool);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Gt(x => x.BoolProperty, datasetValue));
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Resolve_BoolLessThan_ShouldRender(bool datasetValue, string queryValue)
    {
        var sut = new SimpleResolver<DemoClass, bool>(x => x.BoolProperty);
        var filter = sut.Resolve(QueryOperation.LessThan, queryValue, AnQLValueType.Bool);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Lt(x => x.BoolProperty, datasetValue));
    }
    
    #endregion
}