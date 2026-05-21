namespace mbt.webapi.Endpoints.BudgetStructure.dto;

public class SetSponsorshipRequest : ObjectIdRequest
{
    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }
}
