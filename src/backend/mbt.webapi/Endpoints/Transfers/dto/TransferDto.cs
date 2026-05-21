using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.Transfers.dto;

[PublicAPI]
public class TransferDto : BaseItemDto
{
    public string FromBudgetId { get; set; }
    public string ToBudgetId { get; set; }
    public string FromQuarter { get; set; }
    public string ToQuarter { get; set; }
    public double Amount { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }

    public int? Year { get; set; }
    public string Plan { get; set; }

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
