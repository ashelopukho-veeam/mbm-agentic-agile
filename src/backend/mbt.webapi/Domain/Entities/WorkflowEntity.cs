using System;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class WorkflowEntity : BaseIdItem
{
    public string Description { get; set; }
    public string Reference { get; set; }
    public string WorkflowDefinitionId { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? CompleteTime { get; set; }
}
