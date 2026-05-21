using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.Transfers;

public class CreateTransferCommand : IRequest<Transfer>
{
    public CreateTransferCommand(CreateTransferRequest request)
    {
        Request = request;
    }

    public CreateTransferRequest Request { get; }
}

public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, Transfer>
{
    private readonly IApiService _apiService;
    private readonly ITransfersRepository _transfersRepository;
    private readonly ITransfersService _transfersService;
    private readonly IValidator<IWithPaidMediaData> _paidMediaFieldsValidator;

    public CreateTransferCommandHandler(
        IApiService apiService,
        ITransfersRepository transfersRepository,
        ITransfersService transfersService, IValidator<IWithPaidMediaData> paidMediaFieldsValidator)
    {
        _apiService = apiService;
        _transfersRepository = transfersRepository;
        _transfersService = transfersService;
        _paidMediaFieldsValidator = paidMediaFieldsValidator;
    }


    public async Task<Transfer> Handle(CreateTransferCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (request.FromBudgetId == request.ToBudgetId)
        {
            throw new ValidationException(ErrorMessages.TransferFromAndToBudgetsAreTheSame);
        }

        var config = await _apiService.GetCommonConfigAsync();
        if (config.AllowCreateTransfers == false)
            throw new ApiException(ErrorMessages.CreatingTransfersIsNotAllowed);

        var currentTransfersPeriod = await _apiService.GetLastFinalizedPeriod();

        var fromBudget =
            await _transfersService.ValidateBudgetForTransfer(request.FromBudgetId, currentTransfersPeriod);
        var toBudget = await _transfersService.ValidateBudgetForTransfer(request.ToBudgetId, currentTransfersPeriod);

        var transfer = new Transfer
        {
            Title = request.Title,
            FromBudgetId = request.FromBudgetId,
            ToBudgetId = request.ToBudgetId,
            FromQuarter = request.FromQuarter,
            ToQuarter = request.ToQuarter,
            Amount = request.Amount,
            Comment = request.Comment,
            Status = TransferStatus.Draft,
            Year = currentTransfersPeriod.Year,
            Plan = currentTransfersPeriod.PlanName
        };

        var isTransferBetweenPaidMedia = fromBudget.IsPaidMedia || toBudget.IsPaidMedia;
        if (isTransferBetweenPaidMedia)
        {
            await _paidMediaFieldsValidator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        }

        PaidMediaHelper.SetPaidMediaFields(transfer, isTransferBetweenPaidMedia ? request : null);


        transfer = await _transfersRepository.CreateAsync(transfer);

        return transfer;
    }
}
