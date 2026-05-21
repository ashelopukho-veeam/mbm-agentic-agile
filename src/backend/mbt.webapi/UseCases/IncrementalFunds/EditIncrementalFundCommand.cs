using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.IncrementalFunds;

public class EditIncrementalFundCommand : IRequest<IncrementalFund>
{
    public EditIncrementalFundCommand(UpdateIncrementalFundRequest request)
    {
        Request = request;
    }

    public UpdateIncrementalFundRequest Request { get; }
}

public class EditIncrementalFundCommandHandler : IRequestHandler<EditIncrementalFundCommand, IncrementalFund>
{
    private readonly IIncrementalFundsRepository _incrementalFundsRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IIncrementalFundsService _incrementalFundsService;

    public EditIncrementalFundCommandHandler(
        IIncrementalFundsRepository incrementalFundsRepository,
        ICurrentUserContext currentUserContext,
        IIncrementalFundsService incrementalFundsService)
    {
        _incrementalFundsRepository = incrementalFundsRepository;
        _currentUserContext = currentUserContext;
        _incrementalFundsService = incrementalFundsService;
    }

    public async Task<IncrementalFund> Handle(EditIncrementalFundCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var incrementalFund = await _incrementalFundsRepository.GetAsync(request.Id);

        if (incrementalFund == null)
            throw new ApiException("Incremental Fund not found");

        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins });
        var isAuthor = _currentUserContext.UserId == incrementalFund.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        if (incrementalFund.Status != IncrementalFundStatus.Draft)
            throw new ApiException("Update available only for Incremental Funds with Draft status");

        var toBudget =
            await _incrementalFundsService.ValidateAndFetchBudgetAsync(request.ToBudgetId);

        incrementalFund.Title = request.Title;
        incrementalFund.ToBudgetId = toBudget.Id;
        incrementalFund.ToQuarter = request.ToQuarter;
        incrementalFund.Amount = request.Amount;
        incrementalFund.Description = request.Description;

        await _incrementalFundsService.ValidateAndSetPaidMediaFieldsAsync(request, incrementalFund,
            toBudget.IsPaidMedia, cancellationToken);

        await _incrementalFundsRepository.UpdateAsync(incrementalFund);

        return incrementalFund;
    }
}
