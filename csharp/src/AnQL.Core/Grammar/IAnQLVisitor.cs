namespace AnQL.Core.Grammar;

public interface IAnQLVisitor<T> : IAnQLGrammarVisitor<T>
{
    T SuccessQueryResult { get; }
    T FailedQueryResult { get; }
}