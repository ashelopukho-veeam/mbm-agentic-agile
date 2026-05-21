using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Endpoints.PaidMediaSet;

[PublicAPI]
public class EditPaidMediaSetRequest
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string PaidMediaSetType { get; set; }

    public string LinkedItemId { get; set; }

    public string Comment { get; set; }

    public IFormFile FormFile { get; set; }
}

[UsedImplicitly]
public class EditPaidMediaSetRequestValidator : AbstractValidator<EditPaidMediaSetRequest>
{
    public EditPaidMediaSetRequestValidator()
    {
        RuleFor(v => v.Id).IsObjectId();

        RuleFor(v => v.Title).NotEmpty().MaximumLength(ValidationRulesConstants.TitleMaxLength);

        var conditions = new List<string>()
            { PaidMediaSetTypes.Delta, PaidMediaSetTypes.Transfer, PaidMediaSetTypes.IncrementalFund };
        RuleFor(x => x.PaidMediaSetType)
            .Must(x => conditions.Contains(x))
            .WithMessage("Please only use: " + string.Join(",", conditions));

        RuleFor(v => v.LinkedItemId).IsObjectId();

        RuleFor(v => v.Comment)
            .MaximumLength(ValidationRulesConstants.MaxCommentLength)
            .NotEmpty();
    }
}
