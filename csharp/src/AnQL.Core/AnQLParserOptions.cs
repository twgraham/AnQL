namespace AnQL.Core;

public class AnQLParserOptions
{
    public bool FullTextSearch { get; set; } = false;
    public UnknownPropertyBehaviour UnknownPropertyBehaviour { get; set; } = UnknownPropertyBehaviour.Ignore;
}