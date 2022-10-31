namespace AnQL.Core.Resolvers;

public interface IAnQLPropertyResolver<out T>
{
    T Resolve(QueryOperation op, string value, AnQLValueType valueType);
}
