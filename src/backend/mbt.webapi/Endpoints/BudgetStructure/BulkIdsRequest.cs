using System.Collections.Generic;
using FluentValidation;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class BulkIdsRequest
{
    public List<string> Ids { get; set; }
}


public class BulkIdsRequestValidator : AbstractValidator<BulkIdsRequest>
{
    public BulkIdsRequestValidator()
    {
        RuleFor(x => x.Ids).ForEach(t => t.IsObjectId());
    }
}
