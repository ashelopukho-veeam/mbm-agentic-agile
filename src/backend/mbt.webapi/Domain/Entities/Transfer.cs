using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class Transfer : BaseItem, IWithPaidMediaData
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string FromBudgetId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string ToBudgetId { get; set; }

    public string FromQuarter { get; set; }
    public string ToQuarter { get; set; }

    public double Amount { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }

    //
    public int? Year { get; set; }
    public string Plan { get; set; }

    // for internal use
    public string WorkflowId { get; set; }

    public bool IsExpired { get; set; }

    #region Paid media fields

    public string GlobalRegion { get; set; }
    public string SubRegion { get; set; }
    public string GlobalProgram { get; set; }
    public string Team { get; set; }
    public string AdService { get; set; }
    public string Management { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }

    #endregion Paid media fields
}

public static class TransferStatus
{
    public const string Draft = "Draft";
    public const string Canceled = "Canceled";
    public const string WaitingManagersApprove = "Waiting for Managers Approve";
    public const string WaitingOwnersApprove = "Waiting for Owners Approve";
    public const string Rejected = "Rejected";
    public const string Approved = "Approved";
    public const string PendingApproval = "Pending Approval";
}

public static class TransferAction
{
    public const string Cancel = "Cancel";
    public const string Submit = "Submit";
}
