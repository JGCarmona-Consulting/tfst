using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Domain.Repositories.Query;

public interface IUserProfileViewQueryRepository : IQueryRepository<UserProfileView>
{
    Task<UserProfileView?> GetByDisplayNameAsync(string displayName);
    Task<IEnumerable<UserProfileView>> GetByAccountIdAsync(string accountId);
}
