namespace AnQL.Core;

public class UnknownPropertyException : Exception
{
    public string Property { get; }
    
    public UnknownPropertyException(string propertyName) : base($"The property '{propertyName}' is unknown")
    {
        Property = propertyName;
    }
}