using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.PaidMediaSet.dto;
using mbt.webapi.Repositories;
using mbt.webapi.UseCases.PaidMediaSet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class List : EndpointBaseAsync.WithoutRequest.WithActionResult<List<PaidMediaSetDto>>
{
    private readonly IMapper _mapper;
    private readonly IPaidMediaSetRepository _paidMediaSetRepository;

    public List(IMapper mapper, IPaidMediaSetRepository paidMediaSetRepository)
    {
        _mapper = mapper;
        _paidMediaSetRepository = paidMediaSetRepository;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpGet("api/paidMediaSet")]
    [SwaggerOperation(
        Summary = "List Paid Media Sets",
        Description = "List Paid Media Sets",
        OperationId = "PaidMediaSet.List",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult<List<PaidMediaSetDto>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var paidMediaSetItems = await _paidMediaSetRepository.GetExpanded();

        return paidMediaSetItems.Select(p =>
            _mapper.Map<PaidMediaSetWithLinkedItem, PaidMediaSetDto>(p)).ToList();
    }
}
