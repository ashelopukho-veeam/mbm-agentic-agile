using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.IncrementalFunds;

public class CreateIncrementalFundCommand : IRequest<IncrementalFund>
{
    public CreateIncrementalFundCommand(CreateIncrementalFundRequest request)
    {
        Request = request;
    }

    public CreateIncrementalFundRequest Request { get; }
}

public class CreateIncrementalFundCommandHandler : IRequestHandler<CreateIncrementalFundCommand, IncrementalFund>
{
    private readonly IApiService _apiService;
    private readonly IIncrementalFundsRepository _incrementalFundsRepository;
    private readonly IIncrementalFundsService _incrementalFundsService;

    public CreateIncrementalFundCommandHandler(
        IApiService apiService,
        IIncrementalFundsRepository incrementalFundsRepository,
        IIncrementalFundsService incrementalFundsService)
    {
        _apiService = apiService;
        _incrementalFundsRepository = incrementalFundsRepository;
        _incrementalFundsService = incrementalFundsService;
    }

    public async Task<IncrementalFund> Handle(CreateIncrementalFundCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var config = await _apiService.GetCommonConfigAsync();
        if (config.AllowCreateTransfers == false)
            throw new ApiException(ErrorMessages.CreatingTransfersIsNotAllowed);

        var lastFinalizedPeriod = await _apiService.GetLastFinalizedPeriod();

        var toBudget = await _incrementalFundsService.ValidateAndFetchBudgetAsync(request.ToBudgetId);

        var incrementalFund = new IncrementalFund
        {
            Title = request.Title,
            ToBudgetId = toBudget.Id,
            ToQuarter = request.ToQuarter,
            Amount = request.Amount,
            Description = request.Description,
            Status = IncrementalFundStatus.Draft,
            Year = lastFinalizedPeriod.Year,
            Plan = lastFinalizedPeriod.PlanName,
        };

        await _incrementalFundsService.ValidateAndSetPaidMediaFieldsAsync(request, incrementalFund,
            toBudget.IsPaidMedia, cancellationToken);

        await _incrementalFundsRepository.CreateAsync(incrementalFund);

        return incrementalFund;
    }
}
