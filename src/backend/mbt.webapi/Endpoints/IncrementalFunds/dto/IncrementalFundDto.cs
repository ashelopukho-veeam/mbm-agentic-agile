namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

public class IncrementalFundDto : BaseItemDto
{
    public string ToBudgetId { get; set; }
    public int ToQuarter { get; set; }
    public double Amount { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }

    public int Year { get; set; }
    public string Plan { get; set; }

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
