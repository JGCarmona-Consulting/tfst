using TheFullStackTeam.Application.Auth.Models;
using TheFullStackTeam.Domain.Entities;

namespace TheFullStackTeam.Application.Auth.Services;
public interface ITokenService
{
    Task<TokenResponse> GenerateTokens(Account account);
}
