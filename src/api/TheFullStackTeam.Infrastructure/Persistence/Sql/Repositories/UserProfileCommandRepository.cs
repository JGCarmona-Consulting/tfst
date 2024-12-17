using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Repositories;

public class UserProfileCommandRepository : CommandRepository<UserProfile>, ICommandRepository<UserProfile>
{
    public UserProfileCommandRepository(ApplicationDbContext context) : base(context)
    {
    }
}
