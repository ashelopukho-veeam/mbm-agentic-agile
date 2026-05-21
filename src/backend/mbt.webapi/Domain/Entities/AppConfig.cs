using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class AppConfig : BaseIdItem
{
    public string ClientHostUrl { get; set; }
}
