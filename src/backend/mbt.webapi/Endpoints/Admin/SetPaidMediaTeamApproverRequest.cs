using System;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;

namespace mbt.webapi.Endpoints.Admin;

[PublicAPI]
public class SetPaidMediaTeamApproverRequest
{
    public string Team { get; set; }

    public Guid ApproverId { get; set; }
}

[UsedImplicitly]
public class SetPaidMediaTeamApproverRequestValidator : AbstractValidator<SetPaidMediaTeamApproverRequest>
{
    public SetPaidMediaTeamApproverRequestValidator(IUserService userService) {
        RuleFor(v => v.Team)
            .NotEmpty()
            .MaximumLength(ValidationRulesConstants.TitleMaxLength);
        RuleFor(v => v.ApproverId)
            .MustAsync((id, token) => ValidationRules.IsUserExists(userService, id, token))
            .WithMessage((r) => $"User with id {r.ApproverId} does not exist");
    }
}
