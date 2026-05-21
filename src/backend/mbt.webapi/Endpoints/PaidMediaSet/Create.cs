using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.PaidMediaSet.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.UseCases.PaidMediaSet;
using mbt.webapi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using PaidMediaDetails = mbt.webapi.Domain.Entities.PaidMediaDetails;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class Create : EndpointBaseAsync.WithRequest<CreatePaidMediaSetRequest>.WithActionResult<PaidMediaSetDto>
{
    private readonly IPaidMediaSetService _paidMediaSetService;
    private readonly IValidator<CreatePaidMediaSetRequest> _validator;
    private readonly IPaidMediaSetRepository _paidMediaSetRepository;
    private readonly IMapper _mapper;


    public Create(IPaidMediaSetService paidMediaSetService, IValidator<CreatePaidMediaSetRequest> validator,
        IMapper mapper, IPaidMediaSetRepository paidMediaSetRepository)
    {
        _paidMediaSetService = paidMediaSetService;
        _validator = validator;
        _mapper = mapper;
        _paidMediaSetRepository = paidMediaSetRepository;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpPost("api/paidMediaSet")]
    [SwaggerOperation(
        Summary = "Create a Paid Media Set",
        Description = "Create a Paid Media Set",
        OperationId = "PaidMediaSet.Create",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult<PaidMediaSetDto>> HandleAsync([FromForm] CreatePaidMediaSetRequest request,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var paidMediaSetDetails =
            CsvUtils<PaidMediaDetails>.GetCsvDataFromFile(request.FormFile);

        var paidMediaSet = new Domain.Entities.PaidMediaSet
        {
            Comment = request.Comment,
            Details = paidMediaSetDetails,
            IsDeleted = false,
            LinkedItemId = request.LinkedItemId,
            PaidMediaSetType = request.PaidMediaSetType,
            Title = request.Title
        };

        await _paidMediaSetService.CreateAsync(paidMediaSet);

        var createdItem = await _paidMediaSetRepository.GetByIdExpanded(paidMediaSet.Id);

        return _mapper.Map<PaidMediaSetWithLinkedItem, PaidMediaSetDto>(createdItem);
    }
}
