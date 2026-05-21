using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Repositories;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Queries;

public class BudgetStructureGetYearsQuery : IRequest<List<int>>
{
}

public class BudgetStructureGetYearsQueryHandler : IRequestHandler<BudgetStructureGetYearsQuery, List<int>>
{
    private readonly IBudgetRepository _budgetsRepository;

    public BudgetStructureGetYearsQueryHandler(IBudgetRepository budgetsRepository)
    {
        _budgetsRepository = budgetsRepository;
    }

    public async Task<List<int>> Handle(BudgetStructureGetYearsQuery request, CancellationToken cancellationToken)
    {
        var result = await _budgetsRepository.GetDistinctYears(cancellationToken);
        return result;
    }
}
