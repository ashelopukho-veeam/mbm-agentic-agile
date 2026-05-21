using mbt.webapi.Domain.Entities;
using mbt.webapi.Shared;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.Transfers;

public static class Specifications
{
    public static FilterDefinition<Budget> BudgetForTransfer(string budgetId, Period transferPeriod)
    {
        return Builders<Budget>.Filter.Where(b =>
            b.Id == budgetId &&
            b.Year == transferPeriod.Year &&
            b.Status == BudgetStatus.Approved);
    }
}
