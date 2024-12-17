using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NUlid;
using System.Linq.Expressions;
using TheFullStackTeam.Domain.Repositories.Query;
using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Queries;

public class MongoQueryRepository<T> : IQueryRepository<T> where T : BaseView
{
    protected readonly IMongoCollection<T>? _collection;
    protected readonly ILogger<MongoQueryRepository<T>> _logger;

    public MongoQueryRepository(MongoDbWrapper mongoDbWrapper, string collectionName, ILogger<MongoQueryRepository<T>> logger)
    {
        _logger = logger;

        if (mongoDbWrapper.Database != null)
        {
            _collection = mongoDbWrapper.Database.GetCollection<T>(collectionName);
        }
        else
        {
            _logger.LogWarning($"MongoDB is not available. Query repository for {typeof(T).Name} will not be functional.");
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
}
