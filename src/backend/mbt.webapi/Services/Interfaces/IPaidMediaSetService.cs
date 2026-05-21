using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Services.Interfaces;

public interface IPaidMediaSetService : IBaseService
{
    Task RemoveAsync(PaidMediaSet obj);
    Task<PaidMediaSet> CreateAsync(PaidMediaSet paidMediaSet);
    Task<PaidMediaSet> UpdateAsync(PaidMediaSet paidMediaSet);
    Task<PaidMediaSet> GetAsync(string id);
    Task<List<PaidMediaSet>> GetAsync();
    Task<BaseItem> GetLinkedItem(PaidMediaSet paidMediaSet);
}
