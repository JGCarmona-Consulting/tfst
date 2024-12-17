using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TheFullStackTeam.Domain.Repositories.Full;
using TheFullStackTeam.Domain.Views;
using TheFullStackTeam.Infrastructure.Repositories.MongoDB.Repositories;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Full;

public class AccountViewRepository : MongoRepository<AccountView>, IAccountViewRepository
{
    public AccountViewRepository(MongoDbWrapper mongoDbWrapper, ILogger<MongoRepository<AccountView>> logger)
        : base(mongoDbWrapper, "Accounts", logger)
    {
    }

    public async Task<AccountView?> GetByNameAsync(string name)
    {
        if (_collection == null) return null;
        var filter = Builders<AccountView>.Filter.Eq(u => u.LoginName, name);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
