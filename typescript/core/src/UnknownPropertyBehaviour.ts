enum UnknownPropertyBehaviour
{
    /**
     * Should throw on the parse method
     */
    Throw,
    
    /**
     * Should not throw, but return a parsed result that evaluates to false for all inputs
     */
    Fail,
    
    /**
     * Should ignore unknown results where not a determinant
     * E.g. one: 1 AND two: 2
     * two is unknown, but shouldn't stop one from passing the query
     * therefore "two: 2" should resolve to always true
     */
    Ignore,
    
    /**
     * Should always return unknown results as a PASS
     */
    Pass
}

export default UnknownPropertyBehaviour
