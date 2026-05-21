using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class EditBudgetLevelRequest
{
    [FromRoute] public string Id { get; set; }
    [FromBody] public BudgetLevelDto Item { get; set; }
}

[UsedImplicitly]
public class EditBudgetLevelRequestValidator : AbstractValidator<EditBudgetLevelRequest>
{
    public EditBudgetLevelRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Item).NotNull();
        RuleFor(x => x.Item.Id).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.Id == x.Item.Id)
            .WithMessage("Id in request route does not match Id in item");

        RuleFor(x => x.Item.Title).NotEmpty();
        RuleFor(x => x.Item.ShortTitle).NotEmpty();
        RuleFor(x => x.Item.Level).Must(l => l > 0).WithMessage("Level must be greater than 0");
    }
}
