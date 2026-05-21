using System;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

[PublicAPI]
public class EditBudgetStructureRequest : ObjectIdRequest
{
    public string BudgetType { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string CostCenter { get; set; }
    public int Year { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? ParentManagerId { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsPaidMedia { get; set; }
}

[UsedImplicitly]
public class EditBudgetStructureRequestValidator : ObjectIdRequestBaseValidator<EditBudgetStructureRequest>
{
    public EditBudgetStructureRequestValidator(IApiService apiService,
        IDbBaseRepository<DictionaryDocument> dictionaryRepository, IUserService userService)
    {

        RuleFor(b => b.Year)
            .NotEmpty().GreaterThanOrEqualTo(2000)
            .WithMessage("Year must be greater than 2000");

        RuleFor(b => b.BudgetType)
            .NotEmpty().MustAsync((t, token) => BudgetStructureValidators.IsBudgetTypeExists(dictionaryRepository, t, token))
            .WithMessage("Unknown budget type");

        RuleFor(b => b.CostCenter)
            .NotEmpty().MustAsync((c, token) => BudgetStructureValidators.IsCostCenterExists(apiService, c, token))
            .WithMessage("Unknown cost center");

        RuleFor(budget => budget.Level1)
            .NotEmpty().MustAsync((l, t) => BudgetStructureValidators.IsLevelExists(apiService, l, 1))
            .WithMessage("Level1 doesn't exists");

        RuleFor(budget => budget.Level2)
            .NotEmpty().MustAsync((l, t) => BudgetStructureValidators.IsLevelExists(apiService, l, 2))
            .WithMessage("Level2 doesn't exists");

        RuleFor(budget => budget.Level3)
            .NotEmpty().MustAsync((l, t) => BudgetStructureValidators.IsLevelExists(apiService, l, 3))
            .WithMessage("Level3 doesn't exists");

        RuleFor(budget => budget.OwnerId)
            .NotEmpty().MustAsync((u, t) => BudgetStructureValidators.IsUserExists(userService, u, t))
            .WithMessage(ExceptionMessages.UserNotFound);

        RuleFor(budget => budget.ParentManagerId)
            .NotEmpty().MustAsync((u, t) => BudgetStructureValidators.IsUserExists(userService, u, t))
            .WithMessage(ExceptionMessages.UserNotFound);

        RuleFor(budget => budget.ManagerId)
            .NotEmpty().MustAsync((u, t) => BudgetStructureValidators.IsUserExists(userService, u, t))
            .WithMessage(ExceptionMessages.UserNotFound);
    }
}
