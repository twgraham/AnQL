namespace AnQL.Core;

public enum UnknownPropertyBehaviour
{
    /// <summary>
    /// Should throw on the parse method
    /// </summary>
    Throw,
    
    /// <summary>
    /// Should not throw, but return a parsed result that evaluates to false for all inputs
    /// </summary>
    Fail,
    
    /// <summary>
    /// Should ignore unknown results where not a determinant
    /// E.g. one: 1 AND two: 2
    /// two is unknown, but shouldn't stop one from passing the query
    /// therefore "two: 2" should resolve to always true
    /// </summary>
    Ignore,
    
    /// <summary>
    /// Should always return unknown results as a PASS
    /// </summary>
    Pass
}