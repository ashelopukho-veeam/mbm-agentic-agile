using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.Transfers;

public class EditTransferCommand : IRequest<Transfer>
{
    public EditTransferCommand(EditTransferRequest request)
    {
        Request = request;
    }

    public EditTransferRequest Request { get; }
}

public class EditTransferCommandHandler : IRequestHandler<EditTransferCommand, Transfer>
{
    private readonly IApiService _apiService;
    private readonly ITransfersRepository _transfersRepository;
    private readonly ICurrentUserContext _userContext;
    private readonly ITransfersService _transfersService;
    private readonly IValidator<IWithPaidMediaData> _paidMediaFieldsValidator;

    public EditTransferCommandHandler(
        IApiService apiService,
        ITransfersRepository transfersRepository,
        ICurrentUserContext userContext,
        ITransfersService transfersService, IValidator<IWithPaidMediaData> paidMediaFieldsValidator)
    {
        _apiService = apiService;
        _transfersRepository = transfersRepository;
        _userContext = userContext;
        _transfersService = transfersService;
        _paidMediaFieldsValidator = paidMediaFieldsValidator;
    }

    public async Task<Transfer> Handle(EditTransferCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var currentTransfersPeriod = await _apiService.GetLastFinalizedPeriod();

        var fromBudget =
            await _transfersService.ValidateBudgetForTransfer(request.FromBudgetId, currentTransfersPeriod);
        var toBudget =
            await _transfersService.ValidateBudgetForTransfer(request.ToBudgetId, currentTransfersPeriod);

        var transfer = await _transfersRepository.GetAsync(request.Id);
        if (transfer == null)
            throw new ApiException(ErrorMessages.ItemNotFound("Transfer"));

        var isAdmin = _userContext.IsInRole(AppRoles.SysAdmins);
        var isAuthor = _userContext.UserId == transfer.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        if (transfer.Status != TransferStatus.Draft) throw new ApiException("Transfer is not in draft status");

        transfer.Title = request.Title;
        transfer.FromBudgetId = request.FromBudgetId;
        transfer.ToBudgetId = request.ToBudgetId;
        transfer.FromQuarter = request.FromQuarter;
        transfer.ToQuarter = request.ToQuarter;
        transfer.Amount = request.Amount;
        transfer.Comment = request.Comment;

        var isTransferBetweenPaidMedia = fromBudget.IsPaidMedia || toBudget.IsPaidMedia;

        if (isTransferBetweenPaidMedia)
        {
            await _paidMediaFieldsValidator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        }

        PaidMediaHelper.SetPaidMediaFields(transfer, isTransferBetweenPaidMedia ? request : null);

        transfer = await _transfersRepository.UpdateAsync(transfer);
        return transfer;
    }
}
