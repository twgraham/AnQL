namespace AnQL.Core;

public interface IAnQLBuilder<out TParser, TValue> where TParser : IAnQLParser<TValue>
{
    TParser Build();
}