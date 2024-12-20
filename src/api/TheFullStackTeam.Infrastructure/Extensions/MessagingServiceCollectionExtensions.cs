﻿using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Infrastructure.Messaging;
using TheFullStackTeam.Infrastructure.Messaging.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TheFullStackTeam.Infrastructure.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingListener(this IServiceCollection services, IConfiguration configuration)
    {
        var featureFlags = configuration.GetSection("FeatureFlags").Get<FeatureFlags>();

        switch (featureFlags?.MessagingProvider.ToLower())
        {
            case "rabbitmq":
                var rabbitMQSettings = new RabbitMQSettings();
                configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
                services.Configure<RabbitMQSettings>(options =>
                    {
                        options.HostName = rabbitMQSettings.HostName;
                        options.UserName = rabbitMQSettings.UserName;
                        options.Password = rabbitMQSettings.Password;
                        options.QueueName = rabbitMQSettings.QueueName;
                        options.ExchangeName = rabbitMQSettings.ExchangeName;
                    });
                services.AddSingleton<IEventListener, RabbitMQEventListener>();
                break;
            case "azureeventhub":
                // Configuración para Azure Event Hub
                break;
            default:
                throw new InvalidOperationException("Invalid messaging provider configured.");
        }

        return services;
    }
}
