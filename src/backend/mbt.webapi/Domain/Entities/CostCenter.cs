using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class CostCenter : BaseItem
{
    public string ShortTitle { get; set; }
}
