using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Shared;

namespace mbt.webapi.Services.Interfaces;

public interface ITransfersService : IBaseService
{
    Task<List<TransferExpanded>> GetExpanded(int year, string plan);
    Task<TransferExpanded> GetExpanded(string id);
    Task<List<Transfer>> GetTransfersForBudget(string budgetId);
    Task<Transfer> GetAsync(string transferId);
    void Update(Transfer transfer);

    Transfer Get(string transferId);
    Task<Transfer> ExpireTransfer(string requestId);

    Task<Budget> ValidateBudgetForTransfer(string budgetId, Period transferPeriod);
}
