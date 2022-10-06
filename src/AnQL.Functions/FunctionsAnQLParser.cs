using AnQL.Core;
using AnQL.Core.Resolvers;

namespace AnQL.Functions;

public class FunctionAnQLParser<T> : IAnQLParser<Func<T, bool>?>
{
    private readonly AnQLFunctionsVisitor<T> _visitor;
    
    public FunctionAnQLParser(Dictionary<string, IAnQLPropertyResolver<Func<T, bool>>> resolverMap)
    {
        _visitor = new AnQLFunctionsVisitor<T>(resolverMap);
    }
    
    public Func<T, bool>? Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        var anqlParser = Core.AnQL.BuildParser(input);

        return _visitor.Visit(anqlParser.query());
    }
}