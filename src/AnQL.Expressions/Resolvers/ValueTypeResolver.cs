using System.Linq.Expressions;
using AnQL.Core.Resolvers;

namespace AnQL.Expressions.Resolvers;

public class ValueTypeResolver<T, TItem> : IAnQLPropertyResolver<Expression<Func<T, bool>>> where TItem : IComparable<TItem> 
{
    public Expression<Func<T, bool>> Resolve(QueryOperation op, string value, AnQLValueType valueType)
    {
        throw new NotImplementedException();
    }
}