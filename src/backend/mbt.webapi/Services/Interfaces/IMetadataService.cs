using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Services.Interfaces;

public interface IMetadataService : IBaseService
{
    Task<List<TreeItem>> Get();
    Task<TreeItem> Create(TreeItem obj);
    Task Remove(string id);
    Task<TreeItem> Update(string id, TreeItem treeItem);
}
