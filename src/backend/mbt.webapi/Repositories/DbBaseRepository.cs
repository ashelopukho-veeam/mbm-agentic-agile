using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities.Common;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public class DbBaseRepository<T> : IDbBaseRepository<T>
    where T : IBaseIdItem
{
    private readonly ICurrentUserContext _currentUserContext;
    protected IMongoCollection<T> Collection { get; set; }

    public DbBaseRepository(IDbContext context, ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
        Collection = context.GetCollection<T>();
    }

    private FilterDefinition<T> ExcludeDeleted(FilterDefinition<T> filter)
    {
        return IsDeletable(typeof(T))
            ? Builders<T>.Filter.And(filter, Builders<T>.Filter.Ne(nameof(IIsDeleted.IsDeleted), true))
            : filter;
    }

    private void SetAuditableFields(IAuditableItem obj, bool isCreate)
    {
        var nowDate = DateTime.Now;
        obj.Modified = nowDate;
        obj.ModifiedBy = _currentUserContext.UserId;
        if (isCreate)
        {
            obj.Created = nowDate;
            obj.CreatedBy = _currentUserContext.UserId;
        }
        else
        {
            var original = (IAuditableItem)Get(obj.Id);
            obj.Created = original.Created;
            obj.CreatedBy = original.CreatedBy;
        }
    }

    private void SetDeletableFields(IIsDeleted entity, bool isDeleted)
    {
        entity.IsDeleted = isDeleted;
    }


    public T Get(string id)
    {
        return BuildAggregate<T>().Match(entity => entity.Id == id).FirstOrDefault();
    }

    public List<T> Get()
    {
        return BuildAggregate<T>().Match(_ => true).ToList();
    }

    public T Create(T entity)
    {
        if (entity is IAuditableItem auditableItem)
        {
            SetAuditableFields(auditableItem, true);
        }

        Collection.InsertOne(entity);
        return entity;
    }

    public void Update(T entity)
    {
        if (entity is IAuditableItem auditableItem)
        {
            SetAuditableFields(auditableItem, false);
        }

        Collection.ReplaceOne(t => t.Id == entity.Id, entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if (entity is IAuditableItem auditableItem)
        {
            SetAuditableFields(auditableItem, false);
        }

        await Collection.ReplaceOneAsync(t => t.Id == entity.Id, entity);
        return entity;
    }

    public Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update,
        CancellationToken cancellationToken = default)
    {
        // set auditable fields
        if (IsAuditable(typeof(T)))
        {
            update = update.Set(nameof(IAuditableItem.Modified), DateTime.Now)
                .Set(nameof(IAuditableItem.ModifiedBy), _currentUserContext.UserId);
        }

        if (IsDeletable(typeof(T)))
        {
            filter &= Builders<T>.Filter.Ne(nameof(IIsDeleted.IsDeleted), true);
        }

        return Collection.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }

    public IAggregateFluent<TResult> BuildAggregate<TResult>()
    {
        // skip deleted items
        return IsDeletable(typeof(T))
            ? Collection.Aggregate().Match(entity => !((IIsDeleted)entity).IsDeleted).As<TResult>()
            : Collection.Aggregate().As<TResult>();
    }

    public IAggregateFluent<T> BuildAggregate()
    {
        return BuildAggregate<T>();
    }

    public Task<T> FindOneAsync(Expression<Func<T, bool>> filter)
    {
        return BuildAggregate().Match(filter).FirstOrDefaultAsync();
    }

    public Task<T> FindOneAsync(FilterDefinition<T> filter)
    {
        return BuildAggregate().Match(filter).FirstOrDefaultAsync();
    }

    public Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return BuildAggregate().Match(filter).ToListAsync();
    }

    public Task<List<T>> FindAsync(FilterDefinition<T> filter)
    {
        return BuildAggregate().Match(filter).ToListAsync();
    }

    public Task<T> GetAsync(string id)
    {
        return BuildAggregate<T>().Match(entity => entity.Id == id).FirstOrDefaultAsync();
    }

    public Task<List<T>> GetAsync()
    {
        return BuildAggregate<T>().Match(_ => true).ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        if (entity is IAuditableItem auditableItem)
        {
            SetAuditableFields(auditableItem, true);
        }

        await Collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task CreateManyAsync(List<T> entities)
    {
        foreach (var entity in entities)
        {
            if (entity is IAuditableItem auditableItem)
            {
                SetAuditableFields(auditableItem, true);
            }
        }

        await Collection.InsertManyAsync(entities);
    }

    private bool IsDeletable(Type t)
    {
        return t.GetInterface(nameof(IIsDeleted)) != null;
    }

    protected bool IsAuditable(Type t)
    {
        return t.GetInterface(nameof(IAuditableItem)) != null;
    }

    public virtual async Task RemoveAsync(string id)
    {
        if (IsDeletable(typeof(T)))
        {
            var entity = await GetAsync(id);
            SetDeletableFields((IIsDeleted)entity, true);
            await UpdateAsync(entity);
        }
        else
        {
            await Collection.DeleteOneAsync(t => t.Id == id);
        }
    }

    public Task RemoveAsync(IEnumerable<string> id)
    {
        if (!IsDeletable(typeof(T)))
            return Collection.DeleteManyAsync(t => id.Contains(t.Id));

        var entities = GetAsync().Result.Where(t => id.Contains(t.Id)).ToList();
        foreach (var entity in entities)
        {
            SetDeletableFields((IIsDeleted)entity, true);
        }

        return Collection.UpdateManyAsync(t => id.Contains(t.Id),
            Builders<T>.Update.Set(nameof(IIsDeleted.IsDeleted), true));
    }

    public virtual void Remove(string id)
    {
        if (IsDeletable(typeof(T)))
        {
            var entity = Get(id);
            SetDeletableFields((IIsDeleted)entity, true);
            Update(entity);
        }
        else
        {
            Collection.DeleteOne(t => t.Id == id);
        }
    }

    public T FindOne(Expression<Func<T, bool>> filter)
    {
        return BuildAggregate().Match(filter).FirstOrDefault();
    }


    public void Clear()
    {
        ClearAsync().GetAwaiter().GetResult();
    }

    public Task ClearAsync()
    {
        if (IsDeletable(typeof(T)))
        {
            var updateDefinition = Builders<T>.Update.Set(nameof(IIsDeleted.IsDeleted), true);

            if (IsAuditable(typeof(T)))
            {
                updateDefinition = updateDefinition.Set(nameof(IIsDeleted.IsDeleted), true)
                    .Set(nameof(IAuditableItem.Modified), DateTime.Now)
                    .Set(nameof(IAuditableItem.ModifiedBy), _currentUserContext.UserId);
            }

            return Collection.UpdateManyAsync(_ => true, updateDefinition);
        }

        return Collection.DeleteManyAsync(_ => true);
    }

    public long Count()
    {
        return CountAsync().GetAwaiter().GetResult();
    }

    public Task<long> CountAsync()
    {
        return CountAsync(FilterDefinition<T>.Empty);
    }


    public Task<long> CountAsync(FilterDefinition<T> filter)
    {
        var filterDefinition = ExcludeDeleted(filter);
        return Collection.CountDocumentsAsync(filterDefinition);
    }


    public void Remove(Expression<Func<T, bool>> filter, bool force = false)
    {
        if (IsDeletable(typeof(T)) && !force)
        {
            var updateDefinition = Builders<T>.Update.Set(nameof(IIsDeleted.IsDeleted), true);

            if (IsAuditable(typeof(T)))
            {
                updateDefinition = updateDefinition
                    .Set(nameof(IAuditableItem.Modified), DateTime.Now)
                    .Set(nameof(IAuditableItem.ModifiedBy), _currentUserContext.UserId);
            }

            Collection.UpdateMany(filter, updateDefinition);
        }
        else
        {
            Collection.DeleteMany(filter);
        }
    }
}

public static class PagedExtension
{
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(this IAggregateFluent<T> source, int pageStart,
        int pageSize)
    {
        var countFacetName = "count";
        var dataFacetName = "data";

        var countFacet = AggregateFacet.Create(countFacetName,
            PipelineDefinition<T, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

        var dataFacet = AggregateFacet.Create(dataFacetName,
            PipelineDefinition<T, T>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Skip<T>((pageStart - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<T>(pageSize)
            }));

        var aggregation = await source.Facet(countFacet, dataFacet).ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == countFacetName)
            .Output<AggregateCountResult>().First().Count;

        var totalPages = (int)Math.Ceiling((double)count / pageSize);

        var data = aggregation.First()
            .Facets.First(x => x.Name == dataFacetName)
            .Output<T>().ToList();

        return new PagedResult<T> { Items = data, TotalPages = totalPages };
    }
}
