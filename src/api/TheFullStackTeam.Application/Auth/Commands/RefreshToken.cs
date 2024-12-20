using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TheFullStackTeam.Application.Auth.Models;
using TheFullStackTeam.Application.Auth.Services;
using TheFullStackTeam.Domain.Repositories.Command;

namespace TheFullStackTeam.Application.Auth.Commands;
public class RefreshTokenCommand(string token, string refreshToken) : IRequest<TokenResponse>
{
    public string Token { get; set; } = token;
    public string RefreshToken { get; set; } = refreshToken;
}

// TODO: Implement the Result pattern to handle errors
public class RefreshTokenCommandHandler(IRefreshTokenCommandRepository refreshTokenRepository, IAccountCommandRepository accountRepository,
    TokenValidationParameters tokenValidationParameters, ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IRefreshTokenCommandRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IAccountCommandRepository _accountRepository = accountRepository;
    private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParamsClone = _tokenValidationParameters.Clone();
        tokenValidationParamsClone.ValidateLifetime = false;

        try
        {
            // validation: token format is correct
            var tokenVerification = jwtTokenHandler.ValidateToken(
                request.Token,
                tokenValidationParamsClone,
                out var validatedToken);

            // validation: Check encryption
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == false) throw new Exception("The Token has encryption errors");
            }

            // validation: Check expiration date token
            var utcExpiryDateToken = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

            var expiryDateToken = UnixTimeStampToDateTime(utcExpiryDateToken);
            if (expiryDateToken > DateTime.UtcNow) throw new Exception("The Token hasn´t expired");

            //validation: Exist refresh token in DB
            var storedRefreshToken = await _refreshTokenRepository.GetByRefreshTokenAsync(request.RefreshToken) ?? throw new Exception("The refresh Token does not exist");

            // validation to check if the refresh token was already used
            if (storedRefreshToken.IsUsed) throw new Exception("The refresh Token has already been used");

            // validation the refresh token was revoked?
            if (storedRefreshToken.IsRevoked) throw new Exception("The refresh Token has been revoked");

            // check refresh token id
            var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;

            if (storedRefreshToken.JwtId != jti) throw new Exception("Refresh Token does not match initial value");

            // validation for expiration date refresh token
            if (storedRefreshToken.ExpireDate < DateTime.UtcNow) throw new Exception("The refresh token has expired");

            storedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

            var email = tokenVerification.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value;
            var account = await _accountRepository.GetByEmailAsync(email) ?? throw new Exception("No account found for the given token");

            var tokenResponse = await _tokenService.GenerateTokens(account);
            return tokenResponse;

        }
        catch (Exception)
        {
            throw;
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTimeval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTimeval = dateTimeval.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dateTimeval;
    }
}
