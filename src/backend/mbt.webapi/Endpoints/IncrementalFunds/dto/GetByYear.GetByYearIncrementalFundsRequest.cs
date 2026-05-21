using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class GetByYearIncrementalFundsRequest
{
    public int Year { get; set; }
}
