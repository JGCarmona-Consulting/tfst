using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheFullStackTeam.Application.Auth.Models;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;

namespace TheFullStackTeam.Application.Auth.Services;
public class TokenService(IOptions<JwtSettings> jwtOptions, IRefreshTokenCommandRepository refreshTokenRepository) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly IRefreshTokenCommandRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<TokenResponse> GenerateTokens(Account account)
    {
        var selectedProfile = account.Profiles.FirstOrDefault(p => p.IsPrimary) ?? throw new Exception("No primary profile found.");

        // Add custom claims for profile info
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.Email),
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new("ProfileId", selectedProfile?.Id.ToString() ?? string.Empty),
            new("DisplayName", selectedProfile?.DisplayName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add claims for roles
        claims.AddRange(account.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key ?? throw new InvalidOperationException("JWT key not configured."));
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenLifetime = _jwtSettings.TokenLifetimeInHours;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(tokenLifetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(accessToken);

        // Generate refresh token
        var refreshToken = new RefreshToken
        {
            JwtId = accessToken.Id,
            IsUsed = false,
            IsRevoked = false,
            AccountId = account.Id,
            ExpireDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeInDays),
            Token = $"{GenerateRandomTokenCharacters(35)}{Guid.NewGuid()}"
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenResponse
        {
            AccessToken = accessTokenString,
            RefreshToken = refreshToken.Token
        };
    }

    private static string GenerateRandomTokenCharacters(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
                                .Select(_ => chars[random.Next(chars.Length)])
                                .ToArray());
    }
}
