using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TheFullStackTeam.Domain.Repositories.Query;
using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Queries;

public class UserProfileViewQueryRepository : MongoQueryRepository<UserProfileView>, IUserProfileViewQueryRepository
{
    public UserProfileViewQueryRepository(MongoDbWrapper mongoDbWrapper, ILogger<MongoQueryRepository<UserProfileView>> logger)
        : base(mongoDbWrapper, "UserProfiles", logger)
    {
    }

    public async Task<UserProfileView?> GetByDisplayNameAsync(string displayName)
    {
        if (_collection == null) return null;
        var filter = Builders<UserProfileView>.Filter.Eq(u => u.DisplayName, displayName);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetByAccountIdAsync(string accountId)
    {
        if (_collection == null) return [];
        var filter = Builders<UserProfileView>.Filter.Eq(u => u.AccountId, accountId);
        return await _collection.Find(filter).ToListAsync();
    }
}
