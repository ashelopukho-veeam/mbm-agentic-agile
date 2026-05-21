using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services;
using mbt.webapi.UseCases;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using mbt.webapi.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class CopyTo : EndpointBaseAsync.WithRequest<CopyToRequest>.WithResult<BulkOperationResult>
{
    private readonly IValidator<CopyToRequest> _validator;
    private readonly IMediator _mediator;

    public CopyTo(IValidator<CopyToRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost("api/budgetStructure/copyTo")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Copy budget structure to the specific year",
        Description = "Copy budget structure to the specific year",
        OperationId = "BudgetStructure.CopyTo",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<BulkOperationResult> HandleAsync(CopyToRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await BulkOperation.Run(request.Ids,
            async budgetId =>
            {
                var result = await _mediator.Send(
                    new CopyBudgetStructureCommand(budgetId, request.Year, request.Status),
                    cancellationToken);

                if (result.Status != CommandResultStatus.Success)
                {
                    throw new ApiException(result.Message);
                }
            },
            "Copy budget structures");

        return result;
    }
}

[PublicAPI]
public class CopyToRequest
{
    public List<string> Ids { get; set; }
    public int Year { get; set; }
    public string Status { get; set; }
}

[UsedImplicitly]
public class CopyToRequestValidator : AbstractValidator<CopyToRequest>
{
    public CopyToRequestValidator()
    {
        RuleFor(x => x.Ids).NotEmpty()
            .ForEach(t => t.IsObjectId());
        RuleFor(x => x.Year).GreaterThan(ValidationRulesConstants.MinYear);
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => status == BudgetStatus.InProgress || status == BudgetStatus.ApprovedPlaceholder)
            .WithMessage($"Status must be either '{BudgetStatus.InProgress}' or '{BudgetStatus.ApprovedPlaceholder}'");
    }
}
