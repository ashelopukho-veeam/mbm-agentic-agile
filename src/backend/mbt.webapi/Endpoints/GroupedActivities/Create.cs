using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.GA;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class Create : EndpointBaseAsync.WithRequest<AddGroupedActivityToBudgetRequest>.WithResult<GroupedActivityDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IValidator<AddGroupedActivityToBudgetRequest> _validator;


    public Create(IMapper mapper, IMediator mediator, IValidator<AddGroupedActivityToBudgetRequest> validator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _validator = validator;
    }

    [Authorize(Roles = AppRoles.ManageGroupedActivities)]
    [HttpPost("api/groupedActivities")]
    [SwaggerOperation(
        Summary = "Create grouped activity",
        Description = "Create grouped activity",
        OperationId = "GroupedActivities.Create",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<GroupedActivityDto> HandleAsync(AddGroupedActivityToBudgetRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request,cancellationToken);

        var newGroupedActivity = await _mediator.Send(request, cancellationToken);

        var gaDto = _mapper.Map<GroupedActivityDto>(newGroupedActivity);

        return gaDto;
    }
}
