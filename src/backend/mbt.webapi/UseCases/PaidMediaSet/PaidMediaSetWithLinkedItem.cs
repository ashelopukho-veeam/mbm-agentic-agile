using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.UseCases.PaidMediaSet;

public class PaidMediaSetWithLinkedItem : Domain.Entities.PaidMediaSet
{
    public BaseLinkedItem LinkedItem { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}

public class PaidMediaSetWithLinkedItemDto
{
    public BaseLinkedItem LinkedItem { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }

    public string PaidMediaSetType { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string LinkedItemId { get; set; }

    public string Comment { get; set; }
}
