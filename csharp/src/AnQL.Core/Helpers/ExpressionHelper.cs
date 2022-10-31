using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace AnQL.Core.Helpers;

public static class ExpressionHelper
{
    public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var propInfo = GetPropertyInfo(expression);

        var sourceType = typeof(T);
        
        if (sourceType != propInfo.ReflectedType && !sourceType.IsSubclassOf(propInfo.ReflectedType))
            throw new ArgumentException($"Expression '{expression}' refers to a property that is not from type {sourceType}.");

        var displayAttribute = propInfo.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? propInfo.Name;
    }

    public static Type GetPropertyPathType<T>(Expression<Func<T, object>> propertyPath)
    {
        var propertyInfo = GetPropertyInfo(propertyPath);
        return propertyInfo.PropertyType;
    }

    public static LambdaExpression StripConvert(LambdaExpression source)
    {
        var result = source.Body;
        // use a loop in case there are nested Convert expressions for some crazy reason
        while (result.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked
               && result.Type == typeof(object))
        {
            result = ((UnaryExpression)result).Operand;
        }
        return Expression.Lambda(result, source.Parameters);
    }

    private static PropertyInfo GetPropertyInfo<T, TProperty>(Expression<Func<T, TProperty>> propertyPath)
    {
        var body = StripConvert(propertyPath).Body;
        
        var member = body as MemberExpression
                     ?? throw new ArgumentException($"Expression '{propertyPath}' refers to a method, not a property.");
        
        return member.Member as PropertyInfo
                       ?? throw new ArgumentException($"Expression '{propertyPath}' refers to a field, not a property.");
    }
}