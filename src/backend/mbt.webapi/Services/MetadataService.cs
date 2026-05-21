using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MongoDB.Bson;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class MetadataService : IMetadataService
{
    private readonly IDbBaseRepository<TreeItem> _metadataRepository;

    public MetadataService(IDbBaseRepository<TreeItem> metadataRepository)
    {
        _metadataRepository = metadataRepository;
    }

    public Task<List<TreeItem>> Get()
    {
        return _metadataRepository.GetAsync();
    }

    public async Task<TreeItem> Create(TreeItem obj)
    {
        obj.Id = ObjectId.GenerateNewId().ToString();
        await _metadataRepository.CreateAsync(obj);

        return obj;
    }

    public async Task Remove(string id)
    {
        await _metadataRepository.RemoveAsync(id);
    }

    public async Task<TreeItem> Update(string id, TreeItem treeItem)
    {
        await _metadataRepository.UpdateAsync(treeItem);
        return treeItem;
    }
}
