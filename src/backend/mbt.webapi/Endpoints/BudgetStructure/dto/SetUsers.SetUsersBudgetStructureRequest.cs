using System;
using FluentValidation;
using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

[PublicAPI]
public class SetUsersBudgetStructureRequest
{
    public string Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ParentManagerId { get; set; }
    public Guid ManagerId { get; set; }
}

public class SetUsersBudgetStructureRequestValidator : AbstractValidator<SetUsersBudgetStructureRequest>
{
    public SetUsersBudgetStructureRequestValidator()
    {
        RuleFor(x => x.Id).IsObjectId();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ParentManagerId).NotEmpty();
        RuleFor(x => x.ManagerId).NotEmpty();
    }
}
