using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Common.Constants;
using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Events;
using TheFullStackTeam.Domain.Repositories.Command;
using TheFullStackTeam.Domain.Services;
using TheFullStackTeam.Domain.ValueObjects;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Initialization;

public class DatabaseInitializer
{
    private readonly IAccountCommandRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly AdminSettings _adminSettings;

    public DatabaseInitializer(
        IAccountCommandRepository accountRepository,
        IPasswordHasher passwordHasher,
        IEventDispatcher eventDispatcher,
        ILogger<DatabaseInitializer> logger,
        IOptions<AdminSettings> adminSettings)  // Inyectar AdminSettings
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
        _adminSettings = adminSettings.Value;
    }

    public async Task SeedAdminUserAsync()
    {
        var adminUser = await _accountRepository.GetByEmailAsync(_adminSettings.Email!);
        if (adminUser == null)
        {
            adminUser = new Account
            {
                Email = _adminSettings.Email!,
                PasswordHash = _passwordHasher.Hash(_adminSettings.Password!),
                Roles = [UserRoles.Admin, UserRoles.User],
                IsActive = true,
                Profiles =
                {
                    new UserProfile
                    {
                        FirstName = _adminSettings.FirstName!,
                        LastName = _adminSettings.LastName!,
                        DisplayName = _adminSettings.FirstName!,
                        IsPrimary = true
                    }
                }
            };

            await _accountRepository.AddAsync(adminUser);
            _logger.LogInformation("Admin user created successfully in SQL.");

            var emailVerificationToken = VerificationToken.Create();
            var accountCreatedEvent = new AccountCreatedEvent(adminUser, emailVerificationToken);
            await _eventDispatcher.DispatchAsync(accountCreatedEvent);
            _logger.LogInformation("Event dispatched for admin user creation.");

            // Wait 5 seconds:
            await Task.Delay(5000);

            var accountVerifiedEvent = new AccountVerifiedEvent(adminUser.Id);
            await _eventDispatcher.DispatchAsync(accountVerifiedEvent);
            _logger.LogInformation("Event dispatched for admin user verification.");
        }
    }
}
