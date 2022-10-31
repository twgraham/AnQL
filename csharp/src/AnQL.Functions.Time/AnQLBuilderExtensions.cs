using System.Linq.Expressions;
using AnQL.Core;

namespace AnQL.Functions.Time;

public static class AnQLBuilderExtensions
{
    public static IAnQLParserBuilder<Func<T, bool>, T> AddNaturalTime<T>(this IAnQLParserBuilder<Func<T, bool>, T> builder)
    {
        var resolverFactory = new DateTimePropertyResolver<T>.Factory();
        builder.RegisterFactory<DateTimeOffset, DateTimePropertyResolver<T>.Factory>(resolverFactory)
            .RegisterFactory<DateTime, DateTimePropertyResolver<T>.Factory>(resolverFactory);
        
        return builder;
    }

    public static IAnQLParserBuilder<Func<T, bool>, T> WithNaturalDateProperty<T>(this IAnQLParserBuilder<Func<T, bool>, T> builder,
        Expression<Func<T, object>> propertyPath)
    {
        builder.AddNaturalTime();
        return builder.WithProperty(propertyPath);
    }
    
    public static IAnQLParserBuilder<Func<T, bool>, T> WithNaturalDateProperty<T>(this IAnQLParserBuilder<Func<T, bool>, T> builder,
        string name, Func<T, DateTime> propertyAccessor)
    {
        return builder.WithProperty(name, new DateTimePropertyResolver<T>(propertyAccessor));
    }
    
    public static IAnQLParserBuilder<Func<T, bool>, T> WithNaturalDateProperty<T>(this IAnQLParserBuilder<Func<T, bool>, T> builder,
        string name, Func<T, DateTimeOffset> propertyAccessor)
    {
        return builder.WithProperty(name, new DateTimePropertyResolver<T>(propertyAccessor));
    }
}