using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AnQL.Mongo.Tests.Utilities;

public static class FilterDefinitionExtensions
{
    public static string RenderAsJson<T>(this FilterDefinition<T> definition)
    {
        return definition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry).ToJson();
    }
}