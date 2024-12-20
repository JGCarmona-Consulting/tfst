using MediatR;
using TheFullStackTeam.Application.Auth.Models;
using TheFullStackTeam.Application.Auth.Services;
using TheFullStackTeam.Domain.Repositories.Command;
using TheFullStackTeam.Domain.Services;

namespace TheFullStackTeam.Application.Auth.Commands;

public class AuthenticateUserCommand(string username, string password) : IRequest<TokenResponse>
{
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
}

public class AuthenticateUserCommandHandler(
    IAccountCommandRepository accountRepository, IPasswordHasher passwordHasher, ITokenService tokenService
    ) : IRequestHandler<AuthenticateUserCommand, TokenResponse>
{
    private readonly IAccountCommandRepository _accountRepository = accountRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<TokenResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByEmailAsync(request.Username);
        if (account == null || !_passwordHasher.Verify(account.PasswordHash, request.Password))
        {
            throw new Exception($"No account found for email: {request.Username}");
        }

        var tokenResponse = await _tokenService.GenerateTokens(account);
        return tokenResponse;
    }
    
}
