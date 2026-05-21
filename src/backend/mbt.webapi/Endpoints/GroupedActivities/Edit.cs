using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.GA.EditGroupedActivities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class Edit : EndpointBaseAsync.WithRequest<EditGroupedActivityToBudgetRequest>.WithActionResult<
    GroupedActivityDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IValidator<EditGroupedActivityToBudgetRequest> _validator;

    public Edit(IMapper mapper, IMediator mediator, IValidator<EditGroupedActivityToBudgetRequest> validator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _validator = validator;
    }

    [HttpPut("api/groupedActivities")]
    [Authorize(Roles = AppRoles.ManageGroupedActivities)]
    [SwaggerOperation(
        Summary = "Edit grouped activity",
        Description = "Edit grouped activity",
        OperationId = "GroupedActivities.Edit",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<GroupedActivityDto>> HandleAsync(EditGroupedActivityToBudgetRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var ga = await _mediator.Send(request, cancellationToken);

        var gaDto = _mapper.Map<GroupedActivityDto>(ga);

        return gaDto;
    }
}
