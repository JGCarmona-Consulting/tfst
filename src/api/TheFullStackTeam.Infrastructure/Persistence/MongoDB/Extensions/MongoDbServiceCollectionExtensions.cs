using Microsoft.Extensions.DependencyInjection;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB.Extensions;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbWrapper>();
        return services;
    }

    public static async Task InitializeMongoDbAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<MongoDbWrapper>();
        await initializer.EnsureDatabaseIsReady();
    }
}
