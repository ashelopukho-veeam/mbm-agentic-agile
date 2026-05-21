using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class ImportGroupedActivitiesRequest
{
    public string BudgetPlanId { get; set; }

    public IFormFile FormFile { get; set; }
}

public class ImportGroupedActivitiesRequestValidator : AbstractValidator<ImportGroupedActivitiesRequest>
{
    public ImportGroupedActivitiesRequestValidator()
    {
        RuleFor(x => x.BudgetPlanId).IsObjectId();
        RuleFor(x => x.FormFile).NotNull();
    }
}

