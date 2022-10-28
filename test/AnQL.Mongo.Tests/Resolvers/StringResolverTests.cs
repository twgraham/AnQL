using System.Text.RegularExpressions;
using AnQL.Core.Resolvers;
using AnQL.Mongo.Resolvers;
using AnQL.Mongo.Tests.Utilities;
using MongoDB.Driver;
using Xunit;

namespace AnQL.Mongo.Tests.Resolvers;

public class StringResolverTests
{
    [Fact]
    public void Resolve_DefaultRegexDisabled_ShouldRenderExactMatch()
    {
        const string query = "foo";
        var sut = new StringResolver<DemoClass>(x => x.StringProperty);

        var filter = sut.Resolve(QueryOperation.Equal, query, AnQLValueType.String);

        filter.Should().BeEquivalentTo(x => x.StringProperty == query);
    }
    
    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("^foo[a-z]//$")]
    public void Resolve_RegexEnabled_ShouldRenderWithRegex(string queryValue)
    {
        var sut = new StringResolver<DemoClass>(x => x.StringProperty, x =>
        {
            x.RegexMatching = true;
        });

        var filter = sut.Resolve(QueryOperation.Equal, queryValue, AnQLValueType.String);

        filter.Should().BeEquivalentTo(x => Regex.IsMatch(x.StringProperty, queryValue, RegexOptions.None));
    }
    
    [Fact]
    public void Resolve_RegexEnabledWithCaseInsensitive_ShouldRenderWithRegexCaseInsensitive()
    {
        const string query = "foo";
        var sut = new StringResolver<DemoClass>(x => x.StringProperty, x =>
        {
            x.RegexMatching = true;
            x.RegexOptions = RegexOptions.IgnoreCase;
        });

        var filter = sut.Resolve(QueryOperation.Equal, query, AnQLValueType.String);

        filter.Should().BeEquivalentTo(x => Regex.IsMatch(x.StringProperty, query, RegexOptions.IgnoreCase));
    }

    [Fact]
    public void Resolve_LessThan_RegexEnabled_ShouldRenderLessThanWithNoRegex()
    {
        const string query = "foo";
        var sut = new StringResolver<DemoClass>(x => x.StringProperty, x =>
        {
            x.RegexMatching = true;
        });

        var filter = sut.Resolve(QueryOperation.LessThan, query, AnQLValueType.String);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Lt(x => x.StringProperty, query));
    }
    
    [Fact]
    public void Resolve_GreaterThan_RegexEnabled_ShouldRenderLessThanWithNoRegex()
    {
        const string query = "foo";
        var sut = new StringResolver<DemoClass>(x => x.StringProperty, x =>
        {
            x.RegexMatching = true;
        });

        var filter = sut.Resolve(QueryOperation.GreaterThan, query, AnQLValueType.String);

        filter.Should().BeEquivalentTo(Builders<DemoClass>.Filter.Gt(x => x.StringProperty, query));
    }
}