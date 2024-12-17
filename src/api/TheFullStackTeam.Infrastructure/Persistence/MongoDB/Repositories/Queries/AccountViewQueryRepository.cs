using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TheFullStackTeam.Domain.Repositories.Query;
using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Queries;

public class AccountViewQueryRepository : MongoQueryRepository<AccountView>, IAccountViewQueryRepository
{
    public AccountViewQueryRepository(MongoDbWrapper mongoDbWrapper, ILogger<MongoQueryRepository<AccountView>> logger)
        : base(mongoDbWrapper, "Accounts", logger)
    {
    }

    public async Task<AccountView?> GetByNameAsync(string name)
    {
        if (_collection == null) return null;
        var filter = Builders<AccountView>.Filter.Eq(a => a.LoginName, name);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
