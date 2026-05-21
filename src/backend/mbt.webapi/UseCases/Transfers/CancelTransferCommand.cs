using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;
using WorkflowCore.Interface;

namespace mbt.webapi.UseCases.Transfers;

public record CancelTransferCommand(string TransferId) : IRequest<CommandResult<Transfer>>;

public class CancelTransferCommandHandler : IRequestHandler<CancelTransferCommand, CommandResult<Transfer>>
{
    private readonly ITransfersRepository _transfersRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IWorkflowController _workflowController;
    private readonly ITaskService _taskService;
    private readonly IChatService _chatService;

    public CancelTransferCommandHandler(ITransfersRepository transfersRepository,
        ICurrentUserContext currentUserContext, IWorkflowController workflowController, ITaskService taskService,
        IChatService chatService)
    {
        _transfersRepository = transfersRepository;
        _currentUserContext = currentUserContext;
        _workflowController = workflowController;
        _taskService = taskService;
        _chatService = chatService;
    }

    public async Task<CommandResult<Transfer>> Handle(CancelTransferCommand request,
        CancellationToken cancellationToken)
    {
        var transfer = await _transfersRepository.GetAsync(request.TransferId);

        if (transfer == null)
            return CommandResult<Transfer>.NotFound(ErrorMessages.TransferNotFound);

        if (!UserHasPermission(transfer)) throw new AccessDeniedException();
        if (!CanCancelTransfer(transfer)) throw new ApiException(ErrorMessages.TransferCancelWrongStatus);
        await TerminateWorkflowSafely(transfer.WorkflowId);
        await CancelAssociatedTasks(transfer.Id);
        await UpdateTransferStatus(transfer);

        return CommandResult<Transfer>.Success(transfer);
    }

    private bool UserHasPermission(Transfer transfer)
    {
        var isAdmin = _currentUserContext.IsInRoles(AppRoles.Admins, AppRoles.SysAdmins);
        var isAuthor = _currentUserContext.UserId == transfer.CreatedBy;
        return isAdmin || isAuthor;
    }

    private bool CanCancelTransfer(Transfer transfer)
    {
        return transfer.Status != TransferStatus.Approved;
    }

    private async Task TerminateWorkflowSafely(string workflowId)
    {
        if (!string.IsNullOrEmpty(workflowId))
        {
            try
            {
                await _workflowController.TerminateWorkflow(workflowId);
            }
            catch
            {
                // Log the exception or handle specific cases
            }
        }
    }

    private async Task CancelAssociatedTasks(string transferId)
    {
        await _taskService.CancelByAssociatedItemId(transferId);
    }

    private async Task UpdateTransferStatus(Transfer transfer)
    {
        transfer.Status = TransferStatus.Canceled;
        await _transfersRepository.UpdateAsync(transfer);
        await _chatService.AddSystemChatMessageAsync(transfer.Id,
            ChatSystemMessages.TransferSubmitCanceled + _currentUserContext.UserName);
    }
}
