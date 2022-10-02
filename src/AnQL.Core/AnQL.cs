using Antlr4.Runtime;

namespace AnQL.Core;

public static class AnQL
{
    public static AnQLGrammarParser BuildParser(string query)
    {
        return new AnQLGrammarParser(new CommonTokenStream(new AnQLGrammarLexer(new AntlrInputStream(query.ToLower()))));
    }
}