using AnQL.Core.Grammar;

namespace AnQL.Core;

public class AnQLParser<T> : IAnQLParser<T>
{
    private readonly IAnQLVisitor<T> _visitor;

    public AnQLParser(IAnQLVisitor<T> visitor)
    {
        _visitor = visitor;
    }

    public T Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
            return _visitor.SuccessQueryResult;

        var anqlParser = AnQL.BuildParser(input);

        return _visitor.Visit(anqlParser.query()) ?? _visitor.SuccessQueryResult;
    }
}