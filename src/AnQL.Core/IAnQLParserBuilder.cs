namespace AnQL.Core;

public interface IAnQLParserBuilder<out T>
{
    IAnQLParser<T> Build();
}