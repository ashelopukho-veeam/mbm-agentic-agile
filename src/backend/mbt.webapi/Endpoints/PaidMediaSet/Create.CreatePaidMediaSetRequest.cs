using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using Microsoft.AspNetCore.Http;
using PaidMediaSetTypes = mbt.webapi.Domain.Entities.PaidMediaSetTypes;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class CreatePaidMediaSetRequest
{
    public string Title { get; set; }

    public string PaidMediaSetType { get; set; }

    public string LinkedItemId { get; set; }

    public string Comment { get; set; }

    public IFormFile FormFile { get; set; }
}

[UsedImplicitly]
public class CreatePaidMediaSetRequestValidator : AbstractValidator<CreatePaidMediaSetRequest>
{
    public CreatePaidMediaSetRequestValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(ValidationRulesConstants.TitleMaxLength)
            .NotEmpty();

        RuleFor(v => v.LinkedItemId)
            .IsObjectId()
            .NotEmpty();

        var conditions = new List<string>()
            { PaidMediaSetTypes.Delta, PaidMediaSetTypes.Transfer, PaidMediaSetTypes.IncrementalFund };
        RuleFor(x => x.PaidMediaSetType)
            .Must(x => conditions.Contains(x))
            .WithMessage("Please only use: " + string.Join(",", conditions));

        RuleFor(v => v.Comment)
            .MaximumLength(ValidationRulesConstants.MaxCommentLength)
            .NotEmpty();

        RuleFor(v => v.FormFile)
            .NotNull();
    }
}
