using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities.Common;

public abstract class BaseIdItem : IBaseIdItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    protected BaseIdItem()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}

public interface IBaseIdItem
{
    string Id { get; set; }
}

public abstract class BaseStringIdItem : IBaseIdItem
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }

    protected BaseStringIdItem()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}
