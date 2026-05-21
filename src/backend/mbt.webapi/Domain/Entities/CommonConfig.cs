using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class CommonConfig : BaseIdItem
{
    public bool AllowCreateTransfers { get; set; } = true;
    public bool AllowCreateReforecasts { get; set; } = true;
    public bool NotificationBarMessageEnabled { get; set; } = true;
    public string NotificationBarMessage { get; set; } = "";

    public CommonConfig()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}
