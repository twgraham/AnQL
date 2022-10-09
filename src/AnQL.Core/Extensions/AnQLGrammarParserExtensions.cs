using AnQL.Core.Grammar;
using AnQL.Core.Resolvers;

namespace AnQL.Core.Extensions;

public static class AnQLGrammarParserExtensions
{
    public static (string, AnQLValueType) GetValueAndAnQLType(this AnQLGrammarParser.ValueContext valueContext)
    {
        var value = valueContext.GetText();
        return valueContext switch
        {
            AnQLGrammarParser.NullContext => (value, AnQLValueType.Null),
            AnQLGrammarParser.BoolContext => (value, AnQLValueType.Bool),
            AnQLGrammarParser.NumberContext => (value, AnQLValueType.Number),
            AnQLGrammarParser.StringContext => (value, AnQLValueType.String),
            _ => throw new ArgumentOutOfRangeException(nameof(valueContext))
        };
    }
}