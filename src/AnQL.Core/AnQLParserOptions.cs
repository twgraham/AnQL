namespace AnQL.Core;

public class AnQLParserOptions
{
    public bool FullTextSearch { get; set; } = false;
    public bool ThrowOnUnknownProperty { get; set; } = false;
}