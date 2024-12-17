using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;
using TheFullStackTeam.Domain.Repositories.Full;
using TheFullStackTeam.Domain.Repositories.Query;
using TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Full;
using TheFullStackTeam.Infrastructure.Persistence.MongoDB.Repositories.Queries;
using TheFullStackTeam.Infrastructure.Persistence.Sql.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace TheFullStackTeam.Infrastructure.Extensions;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddCommandRepositories(this IServiceCollection services)
    {
        // Command repositories based on SQL
        services.AddScoped<IAccountCommandRepository, AccountCommandRepository>();
        services.AddScoped<ICommandRepository<UserProfile>, UserProfileCommandRepository>();


        return services;
    }

    public static IServiceCollection AddQueryRepositories(this IServiceCollection services)
    {
        // Query repositories based on MongoDB (Account and Question views,containing domain aggregates)
        services.AddScoped<IAccountViewQueryRepository, AccountViewQueryRepository>();
        services.AddScoped<IUserProfileViewQueryRepository, UserProfileViewQueryRepository>();

        return services;
    }

    public static IServiceCollection AddSyncRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountViewRepository, AccountViewRepository>();
        services.AddScoped<IUserProfileViewRepository, UserProfileViewRepository>();
        return services;
    }
}
