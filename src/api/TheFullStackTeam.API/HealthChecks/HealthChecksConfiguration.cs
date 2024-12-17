using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Common.Constants;

namespace TheFullStackTeam.API.HealthChecks;
public static class HealthChecksConfiguration
{
    public static void AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQSettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>() ?? throw new InvalidOperationException("RabbitMQ settings are not configured properly in the application settings.");

        if (string.IsNullOrEmpty(rabbitMQSettings.UserName) || string.IsNullOrEmpty(rabbitMQSettings.Password) || string.IsNullOrEmpty(rabbitMQSettings.HostName))
        {
            throw new InvalidOperationException("RabbitMQ settings are incomplete. Please provide valid UserName, Password, and HostName.");
        }

        services.AddHealthChecks()
            //check liveness
            .AddCheck(HealthCheckTags.Liveness, () => HealthCheckResult.Healthy(), tags: [HealthCheckTags.Liveness])

            //check readiness 
            // SQL Server
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                healthQuery: "SELECT 1",
                name: "SQL Server",
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Readiness])
            // MongoDb
            .AddMongoDb(
                mongodbConnectionString: configuration["MongoDbSettings:ConnectionString"]!,
                mongoDatabaseName: configuration["MongoDbSettings:DatabaseName"]!,
                name: "MongoDB",
                failureStatus: HealthStatus.Degraded,
                tags: [HealthCheckTags.Readiness])
            // RabbitMQ
            .AddRabbitMQ(
                rabbitConnectionString: $"amqp://{rabbitMQSettings!.UserName}:{rabbitMQSettings.Password}@{rabbitMQSettings.HostName}",
                name: "RabbitMQ",
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Readiness])
            ;
    }

}

