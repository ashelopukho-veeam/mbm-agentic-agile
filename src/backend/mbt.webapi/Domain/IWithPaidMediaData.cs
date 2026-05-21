namespace mbt.webapi.Domain;

public interface IWithPaidMediaData
{
    public string GlobalRegion { get; set; }
    public string SubRegion { get; set; }
    public string GlobalProgram { get; set; }
    public string Team { get; set; }
    public string AdService { get; set; }
    public string Management { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }
}
