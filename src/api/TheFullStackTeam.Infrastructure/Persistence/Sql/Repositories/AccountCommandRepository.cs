using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;
using Microsoft.EntityFrameworkCore;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Repositories;

public class AccountCommandRepository : CommandRepository<Account>, IAccountCommandRepository
{
    public AccountCommandRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByEmailAsync(string name)
    {
        return await _context.Accounts
            .Include(a => a.Profiles)
            .FirstOrDefaultAsync(u => u.Email == name);
    }
}
