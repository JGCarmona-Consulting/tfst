using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Domain.Repositories.Full;

public interface IAccountViewRepository : IRepository<AccountView>
{
    Task<AccountView?> GetByNameAsync(string accountname);
}
