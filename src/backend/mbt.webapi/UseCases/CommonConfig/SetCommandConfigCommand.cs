using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using mbt.webapi.Endpoints.CommonConfig;
using mbt.webapi.Services.Budgets;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.CommonConfig;

public class SetCommandConfigCommand : IRequest<Domain.Entities.CommonConfig>
{
    public SetCommonConfigRequest Request { get; init; }
}

public class SetCommandConfigRequestHandler : IRequestHandler<SetCommandConfigCommand, Domain.Entities.CommonConfig>
{
    private readonly IApiService _apiService;
    private readonly IMapper _mapper;
    private readonly IBudgetValidationService _budgetValidationService;

    public SetCommandConfigRequestHandler(IApiService apiService,
        IMapper mapper, IBudgetValidationService budgetValidationService)
    {
        _apiService = apiService;
        _mapper = mapper;
        _budgetValidationService = budgetValidationService;
    }

    public async Task<Domain.Entities.CommonConfig> Handle(SetCommandConfigCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var originalConfig = await _apiService.GetCommonConfigAsync();
        if (originalConfig is { AllowCreateTransfers: true } && !request.AllowCreateTransfers)
        {
            var lastFinalizedPeriod = await _apiService.GetLastFinalizedPeriod();
            await _budgetValidationService.ValidateUnprocessedTransfersAndIncrementalFunds(lastFinalizedPeriod);
        }

        var newConfig = _mapper.Map<Domain.Entities.CommonConfig>(request);
        await _apiService.SetCommonConfigAsync(newConfig);

        return newConfig;
    }
}
