namespace AnQL.Core.Grammar;

public abstract class AnQLBaseVisitor<T> : AnQLGrammarBaseVisitor<T>, IAnQLVisitor<T>
{
    protected AnQLParserOptions Options { get; }
    public abstract T SuccessQueryResult { get; }
    public abstract T FailedQueryResult { get; }

    protected AnQLBaseVisitor(AnQLParserOptions options)
    {
        Options = options;
    }
    
    public override T VisitQuery(AnQLGrammarParser.QueryContext context)
    {
        try
        {
            return base.VisitQuery(context);
        }
        catch (UnknownPropertyException)
        {
            if (Options.UnknownPropertyBehaviour == UnknownPropertyBehaviour.Throw)
                throw;
        }

        return FailedQueryResult;
    }

    protected T HandleUnknownProperty(AnQLGrammarParser.Property_pathContext propertyPathContext)
    {
        if (Options.UnknownPropertyBehaviour is UnknownPropertyBehaviour.Throw or UnknownPropertyBehaviour.Fail)
            throw new UnknownPropertyException(propertyPathContext.GetText());

        if (Options.UnknownPropertyBehaviour is UnknownPropertyBehaviour.Pass)
            return SuccessQueryResult;

        var currentContext = propertyPathContext.Parent.Parent;
        var notSwitch = false;
        while (currentContext != null)
        {
            switch (currentContext)
            {
                case AnQLGrammarParser.ExprANDContext:
                    return notSwitch ? FailedQueryResult : SuccessQueryResult;
                case AnQLGrammarParser.ExprORContext:
                    return notSwitch ? SuccessQueryResult : FailedQueryResult;
                case AnQLGrammarParser.NOTContext:
                    notSwitch = !notSwitch;
                    break;
            }

            currentContext = currentContext.Parent;
        }

        return notSwitch ? FailedQueryResult : SuccessQueryResult;
    }
}