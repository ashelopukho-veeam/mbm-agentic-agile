using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

[PublicAPI]
public class BulkEditRequest
{
    public List<string> Ids { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? ParentManagerId { get; set; }
    public Guid? ManagerId { get; set; }
}

[UsedImplicitly]
public class BulkEditRequestValidator : AbstractValidator<BulkEditRequest>
{
    private readonly IUserService _userService;

    public BulkEditRequestValidator(IUserService userService)
    {
        _userService = userService;

        RuleForEach(v => v.Ids)
            .NotEmpty()
            .Length(BuiltInConstants.ObjectIdLength);

        RuleFor(v => v.OwnerId)
            .MustAsync(IsNullOrUserExists)
            .WithMessage(ExceptionMessages.UserNotFound);
        RuleFor(v => v.ParentManagerId)
            .MustAsync(IsNullOrUserExists)
            .WithMessage(ExceptionMessages.UserNotFound);
        RuleFor(v => v.ManagerId)
            .MustAsync(IsNullOrUserExists)
            .WithMessage(ExceptionMessages.UserNotFound);
    }

    private async Task<bool> IsNullOrUserExists(Guid? userId, CancellationToken token)
    {
        if (userId == null)
            return true;

        var user = await _userService.GetAsync(userId.ToString());
        return user != null;
    }
}
