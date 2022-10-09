namespace AnQL.Core.Grammar;

public interface IAnQLVisitor<T> : IAnQLGrammarVisitor<T>
{
    public T SuccessQueryResult { get; }
    public T FailedQueryResult { get; }
}