using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace AnQL.Core.Helpers;

public static class ExpressionHelper
{
    public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var member = expression.Body as MemberExpression
            ?? throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

        var propInfo = member.Member as PropertyInfo
            ?? throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

        var sourceType = typeof(T);

        if (sourceType != propInfo.ReflectedType && !sourceType.IsSubclassOf(propInfo.ReflectedType))
            throw new ArgumentException($"Expression '{expression}' refers to a property that is not from type {sourceType}.");

        var displayAttribute = propInfo.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? propInfo.Name;
    }
}