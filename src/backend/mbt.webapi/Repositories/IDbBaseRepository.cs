using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IDbBaseRepository<T>
{
    T Get(string id);
    Task<T> GetAsync(string id);
    List<T> Get();
    Task<List<T>> GetAsync();
    T Create(T entity);
    Task<T> CreateAsync(T entity);
    void Update(T entity);
    Task<T> UpdateAsync(T entity);

    Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string id);
    Task RemoveAsync(IEnumerable<string> id);
    void Remove(string id);

    void Remove(Expression<Func<T, bool>> filter, bool force = false);

    void Clear();
    Task ClearAsync();

    Task CreateManyAsync(List<T> entities);

    Task<T> FindOneAsync(Expression<Func<T, bool>> filter);
    Task<T> FindOneAsync(FilterDefinition<T> filter);
    T FindOne(Expression<Func<T, bool>> filter);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task<List<T>> FindAsync(FilterDefinition<T> filter);

    long Count();
    Task<long> CountAsync();
    Task<long> CountAsync(FilterDefinition<T> filter);

    // Task<long> CountAsync(Expression<Func<T, bool>> filter);
}
