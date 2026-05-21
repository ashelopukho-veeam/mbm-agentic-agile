using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Domain.Entities.Common;
using mbt.webapi.Repositories;

namespace mbt.webapi.Services;

public class PaidMediaSetService : IPaidMediaSetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransfersService _transfersService;
    private readonly IIncrementalFundsService _incrementalFundsService;

    private readonly IDbBaseRepository<PaidMediaSet> _paidMediaSetRepository;

    public PaidMediaSetService(
        IIncrementalFundsService incrementalFundsService,
        ITransfersService transfersService,
        IDbBaseRepository<PaidMediaSet> paidMediaSetRepository,
        IBudgetRepository budgetRepository)
    {
        _incrementalFundsService = incrementalFundsService;
        _transfersService = transfersService;
        _paidMediaSetRepository = paidMediaSetRepository;
        _budgetRepository = budgetRepository;
    }

    public Task RemoveAsync(PaidMediaSet obj)
    {
        return _paidMediaSetRepository.RemoveAsync(obj.Id);
    }

    public async Task<PaidMediaSet> CreateAsync(PaidMediaSet paidMediaSet)
    {
        await ValidatePaidMediaSet(paidMediaSet);
        await _paidMediaSetRepository.CreateAsync(paidMediaSet);

        return paidMediaSet;
    }

    public async Task<PaidMediaSet> UpdateAsync(PaidMediaSet paidMediaSet)
    {
        await ValidatePaidMediaSet(paidMediaSet);

        await _paidMediaSetRepository.UpdateAsync(paidMediaSet);

        return paidMediaSet;
    }

    private async Task ValidatePaidMediaSet(PaidMediaSet paidMediaSet)
    {
        if (paidMediaSet.PaidMediaSetType is not
            (PaidMediaSetTypes.Transfer or
            PaidMediaSetTypes.IncrementalFund or
            PaidMediaSetTypes.Delta))
            throw new ApiException($"PaidMediaSetType: {paidMediaSet.PaidMediaSetType} isn't supported");

        if (paidMediaSet.Details.Count == 0)
            throw new ApiException("Paid Media Set doesn't contain Paid Media Details data");

        // validate sum
        var paidMediaDetailsSum = paidMediaSet.Details.Select(p => p.Amount).Sum();
        if (paidMediaDetailsSum != 0.0M) throw new ApiException("The sum of the paid media details must be 0");

        await ValidateLinkedItem(paidMediaSet.PaidMediaSetType, paidMediaSet.LinkedItemId);
    }

    private async Task ValidateLinkedItem(string paidMediaSetType, string linkedItemId)
    {
        switch (paidMediaSetType)
        {
            case PaidMediaSetTypes.Delta:
            {
                var budget = await _budgetRepository.GetAsync(linkedItemId);

                if (budget == null) throw new ApiException($"Budget not found. Id: {linkedItemId}");

                if (!budget.IsPaidMedia)
                    throw new ApiException("Only Paid Media budgets are allowed.");
                break;
            }
            case PaidMediaSetTypes.Transfer:
            {
                var linkedTransfer = await _transfersService.GetAsync(linkedItemId);
                if (linkedTransfer == null) throw new ApiException($"Transfer not found. Id: {linkedItemId}");

                break;
            }
            case PaidMediaSetTypes.IncrementalFund:
            {
                var linkedIncrementalFund = await _incrementalFundsService.GetAsync(linkedItemId);
                if (linkedIncrementalFund == null)
                    throw new ApiException($"Incremental Fund not found. Id: {linkedItemId}");

                break;
            }
        }
    }

    public Task<PaidMediaSet> GetAsync(string id)
    {
        return _paidMediaSetRepository.GetAsync(id);
    }

    public Task<List<PaidMediaSet>> GetAsync()
    {
        return _paidMediaSetRepository.GetAsync();
    }

    public async Task<BaseItem> GetLinkedItem(PaidMediaSet paidMediaSet)
    {
        var linkedItemType = paidMediaSet.PaidMediaSetType;
        BaseItem linkedItem = linkedItemType switch
        {
            PaidMediaSetTypes.Delta => await _budgetRepository.GetAsync(paidMediaSet.LinkedItemId),
            PaidMediaSetTypes.Transfer => await _transfersService.GetAsync(paidMediaSet.LinkedItemId),
            PaidMediaSetTypes.IncrementalFund => await _incrementalFundsService.GetAsync(paidMediaSet.LinkedItemId),
            _ => throw new ApiException($"PaidMediaSetType: {paidMediaSet.PaidMediaSetType} isn't supported")
        };

        return linkedItem;
    }
}
