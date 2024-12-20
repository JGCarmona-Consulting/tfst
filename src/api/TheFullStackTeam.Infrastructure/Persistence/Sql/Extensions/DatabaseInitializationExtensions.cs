﻿using TheFullStackTeam.Infrastructure.Persistence.Sql.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        // Check if the database should be migrated at startup via feature flag
        if (configuration.GetValue<bool>("FeatureFlags:MigrateAtStartup"))
        {
            await migrator.MigrateDatabaseAsync();
        }

        // Seed admin user
        await initializer.SeedAdminUserAsync();
    }
}
