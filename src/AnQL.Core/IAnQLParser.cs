namespace AnQL.Core;

public interface IAnQLParser<out T>
{
    T Parse(string input);
}
