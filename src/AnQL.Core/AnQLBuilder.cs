namespace AnQL.Core;

public class AnQLBuilder
{
    private readonly AnQLParserOptions _options;

    public AnQLBuilder() : this(new AnQLParserOptions())
    {
    }

    public AnQLBuilder(AnQLParserOptions options)
    {
        _options = options;
    }

    public TBuilder For<TBuilder, TReturn, TItem>(Func<AnQLParserOptions, TBuilder> creator)
        where TBuilder : IAnQLParserBuilder<TReturn, TItem>
    {
        return creator(_options);
    }
}
