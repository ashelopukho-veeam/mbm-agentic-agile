using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class UserProfile : BaseStringIdItem
{
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public bool IsGlobalApprover { get; set; }
}
