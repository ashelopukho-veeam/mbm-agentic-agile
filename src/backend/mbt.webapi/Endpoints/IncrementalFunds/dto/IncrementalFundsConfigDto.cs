using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class IncrementalFundsConfigDto
{
    public string WorkflowApproverId { get; set; }
}
