using Microsoft.EntityFrameworkCore;
using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Repositories;
public class RefreshTokenCommandRepository : CommandRepository<RefreshToken>, IRefreshTokenCommandRepository
{
    public RefreshTokenCommandRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken) 
        => await _context.RefreshTokens!.FirstOrDefaultAsync(x => x.Token == refreshToken);

}
