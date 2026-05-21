using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class DeleteIncrementalFundRequest
{
    public string IncrementalFundId { get; set; }
}
