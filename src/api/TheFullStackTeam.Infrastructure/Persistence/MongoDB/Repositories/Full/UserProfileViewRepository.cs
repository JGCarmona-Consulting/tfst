using Microsoft.Extensions.Logging;
using TheFullStackTeam.Domain.Repositories.Full;
using TheFullStackTeam.Domain.Views;
using TheFullStackTeam.Infrastructure.Repositories.MongoDB.Repositories;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Full;

public class UserProfileViewRepository : MongoRepository<UserProfileView>, IUserProfileViewRepository
{
    public UserProfileViewRepository(MongoDbWrapper mongoDbWrapper, ILogger<MongoRepository<UserProfileView>> logger)
        : base(mongoDbWrapper, "UserProfiles", logger)
    {
    }
}
