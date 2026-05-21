using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Services.Interfaces;

public interface IIncrementalFundsService : IBaseService
{
    Task<List<IncrementalFundExpanded>> GetExpanded(int year, string plan);
    Task<IncrementalFundExpanded> GetExpanded(string id);
    Task<List<IncrementalFund>> GetByBudgetIdAsync(string budgetId);
    Task<List<IncrementalFund>> GetByYearAsync(int year);
    IncrementalFund Create(IncrementalFund obj);
    Task<IncrementalFund> CreateAsync(IncrementalFund obj);
    Task<IncrementalFund> GetAsync(string requestId);
    Task UpdateAsync(IncrementalFund incrementalFund);
    Task RemoveAsync(IncrementalFund itemToDelete);
    IncrementalFund Get(string incrementalFundId);
    void Update(IncrementalFund incrementalFund);
    Task SetConfig(IncrementalFundsConfig config);
    Task<IncrementalFundsConfig> GetConfig();
    Task<IncrementalFund> ExpireIncrementalFund(string incrementalFundId);

    Task<IncrementalFund> CancelIncrementalFund(string incrementalFundId);

    //
    Task<Budget> ValidateAndFetchBudgetAsync(string toBudgetId);

    Task ValidateAndSetPaidMediaFieldsAsync(IWithPaidMediaData request, IncrementalFund incrementalFund,
        bool isPaidMedia, CancellationToken cancellationToken);
}
