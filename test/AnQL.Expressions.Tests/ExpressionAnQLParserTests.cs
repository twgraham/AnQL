using System.Linq.Expressions;
using AnQL.Core;
using FluentAssertions;
using Xunit;

namespace AnQL.Expressions.Tests;

public class ExpressionAnQLParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyParser_Parse_ShouldReturnConstantTrueExpression(string query)
    {
        var anqlParser = new AnQLBuilder().ForExpressions<DemoClass>().Build();
        var expression = anqlParser.Parse(query);

        expression.Body.Should().BeOfType<ConstantExpression>().Which.Value.Should().Be(true);
    }

    private class DemoClass
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
}