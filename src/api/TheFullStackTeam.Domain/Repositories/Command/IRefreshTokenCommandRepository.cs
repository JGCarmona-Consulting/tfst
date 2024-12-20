using TheFullStackTeam.Domain.Entities;

namespace TheFullStackTeam.Domain.Repositories.Command;
public interface IRefreshTokenCommandRepository : ICommandRepository<RefreshToken>
{
    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken);
}
