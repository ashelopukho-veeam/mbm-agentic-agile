using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.PaidMediaSet.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.UseCases.PaidMediaSet;
using mbt.webapi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class Edit : EndpointBaseAsync.WithRequest<EditPaidMediaSetRequest>.WithActionResult<PaidMediaSetDto>
{
    private readonly IPaidMediaSetService _paidMediaSetService;
    private readonly IPaidMediaSetRepository _paidMediaSetRepository;
    private readonly IValidator<EditPaidMediaSetRequest> _validator;
    private readonly IMapper _mapper;


    public Edit(IPaidMediaSetService paidMediaSetService, IValidator<EditPaidMediaSetRequest> validator, IMapper mapper,
        IPaidMediaSetRepository paidMediaSetRepository)
    {
        _paidMediaSetService = paidMediaSetService;
        _validator = validator;
        _mapper = mapper;
        _paidMediaSetRepository = paidMediaSetRepository;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpPut("api/paidMediaSet")]
    [SwaggerOperation(
        Summary = "Edit a Paid Media Set",
        Description = "Edit a Paid Media Set",
        OperationId = "PaidMediaSet.Edit",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult<PaidMediaSetDto>> HandleAsync([FromForm] EditPaidMediaSetRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var paidMediaSet = await _paidMediaSetService.GetAsync(request.Id);
        if (paidMediaSet == null) return NotFound();

        List<PaidMediaDetails> paidMediaDetailsList = null;
        if (request.FormFile != null)
            paidMediaDetailsList = CsvUtils<PaidMediaDetails>.GetCsvDataFromFile(request.FormFile);

        paidMediaSet.Comment = request.Comment;
        paidMediaSet.Details = paidMediaDetailsList ?? paidMediaSet.Details;
        paidMediaSet.IsDeleted = false;
        paidMediaSet.LinkedItemId = request.LinkedItemId;
        paidMediaSet.PaidMediaSetType = request.PaidMediaSetType;
        paidMediaSet.Title = request.Title;

        await _paidMediaSetService.UpdateAsync(paidMediaSet);

        var paidMediaSetWithLinkedItem = await _paidMediaSetRepository.GetByIdExpanded(paidMediaSet.Id);


        return _mapper.Map<PaidMediaSetWithLinkedItem, PaidMediaSetDto>(paidMediaSetWithLinkedItem);
    }
}
