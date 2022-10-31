using Antlr4.Runtime;
using AnQL.Core.Grammar;

namespace AnQL.Core;

public static class AnQL
{
    public static AnQLBuilder Builder => new();

    public static AnQLGrammarParser BuildParser(string query)
    {
        return new AnQLGrammarParser(new CommonTokenStream(new AnQLGrammarLexer(new AntlrInputStream(query))));
    }
}