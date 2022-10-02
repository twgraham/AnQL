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

    public TBuilder For<TBuilder, TValue>(Func<AnQLParserOptions, TBuilder> creator)
        where TBuilder : IAnQLParserBuilder<TValue>
    {
        return creator(_options);
    }
}
