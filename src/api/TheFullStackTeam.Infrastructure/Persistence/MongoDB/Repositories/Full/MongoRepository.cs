using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NUlid;
using System.Linq.Expressions;
using TheFullStackTeam.Domain.Repositories.Full;
using TheFullStackTeam.Domain.Views;
using TheFullStackTeam.Infrastructure.Persistence.MongoDB;

namespace TheFullStackTeam.Infrastructure.Repositories.MongoDB.Repositories;

public class MongoRepository<T> : IRepository<T> where T : BaseView
{
    protected readonly IMongoCollection<T>? _collection;
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(MongoDbWrapper mongoDbWrapper, string collectionName, ILogger<MongoRepository<T>> logger)
    {
        _logger = logger;

        if (mongoDbWrapper.Database != null)
        {
            _collection = mongoDbWrapper.Database.GetCollection<T>(collectionName);
        }
        else
        {
            _logger.LogWarning($"MongoDB is not available. Repository for {typeof(T).Name} will not be functional.");
            _collection = null;
        }
    }

    public async Task<T?> GetByIdAsync(Ulid id)
    {
        if (_collection == null) return null;

        var filter = Builders<T>.Filter.Eq("EntityId", id.ToString());
        return await _collection
            .Find(filter)
            .SortByDescending(q => q.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        if (_collection == null) return [];
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (_collection == null) return [];
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        if (_collection == null) return;
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (_collection == null) return;
        var filter = Builders<T>.Filter.Eq("EntityId", entity.GetType().GetProperty("EntityId")?.GetValue(entity, null)?.ToString() ?? string.Empty);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(T entity)
    {
        if (_collection == null) return;
        var filter = Builders<T>.Filter.Eq("EntityId", entity.GetType().GetProperty("EntityId")?.GetValue(entity, null)?.ToString() ?? string.Empty);
        await _collection.DeleteOneAsync(filter);
    }
}
