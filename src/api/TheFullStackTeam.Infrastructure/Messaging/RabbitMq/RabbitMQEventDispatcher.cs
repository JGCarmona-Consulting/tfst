using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Common.Converters;
using TheFullStackTeam.Domain.Events;
using TheFullStackTeam.Domain.Services;

namespace TheFullStackTeam.Infrastructure.Messaging.RabbitMq;
/// <summary>
/// Provides an implementation of <see cref="IEventDispatcher"/> that leverages RabbitMQ as the message broker.
/// 
/// The RabbitMQEventDispatcher is responsible for sending messages to RabbitMQ queues.
/// This implementation uses Lazy initialization for connections and channels to:
/// - Defer resource-heavy operations until they are strictly necessary.
/// - Avoid failures during application startup caused by transient network issues.
/// - Ensure that resources are thread-safe and only initialized once.
/// 
/// Note: Developers must ensure that the connection and channel are properly disposed
/// to avoid resource leaks or unintended behavior during application shutdown.
/// </summary>
public class RabbitMQEventDispatcher : IEventDispatcher
{
    /// <summary>
    /// Represents a lazy-loaded RabbitMQ connection, ensuring it is only initialized when required.
    /// This improves performance and avoids unnecessary resource allocation in cases where the connection
    /// is not immediately needed.
    /// </summary>
    private readonly Lazy<IConnection> _lazyConnection;
    /// <summary>
    /// Represents a lazy-loaded RabbitMQ channel for interacting with queues and exchanges.
    /// It is initialized only when required, ensuring efficient use of resources and supporting
    /// thread-safe operations in concurrent scenarios.
    private readonly Lazy<IModel> _lazyChannel;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQEventDispatcher> _logger;

    public RabbitMQEventDispatcher(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQEventDispatcher> logger)
    {
        _logger = logger;
        _settings = settings.Value;
        _lazyConnection = new Lazy<IConnection>(() =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            return factory.CreateConnection();
        });
        _lazyChannel = new Lazy<IModel>(() =>
        {
            // Create the channel from the connection
            var channel = _lazyConnection.Value.CreateModel();

            // Declare the exchange
            channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: "fanout",  // Use "fanout" to broadcast the message to multiple queues
                durable: true,
                autoDelete: false,
                arguments: null
            );
            return channel;
            // NOTE: bind queues to the exchange in the consumer side
        });
    }

    public Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : EventBase
    {
        var eventType = domainEvent.GetType().AssemblyQualifiedName;
        var messagePayload = new
        {
            EventType = eventType,
            EventData = domainEvent
        };
        var jsonOptions = new JsonSerializerOptions
        {
            Converters = { new UlidJsonConverter() }
        };
        var message = JsonSerializer.Serialize(messagePayload, jsonOptions);
        var body = Encoding.UTF8.GetBytes(message);

        var channel = _lazyChannel.Value;
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = new Dictionary<string, object>
        {
            { "CorrelationId", domainEvent.CorrelationId }
        };

        // retry policy for BrokerUnreachableException and AlreadyClosedException
        var policy = Policy
        .Handle<BrokerUnreachableException>().Or<AlreadyClosedException>()
        .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        (exception, timeSpan, retryCount, context) =>
        {
            _logger.LogWarning(exception,
                "RabbitMQ unreachable. Retry {RetryCount} after {DelaySeconds} seconds. EventType: {EventType}, CorrelationId: {CorrelationId}, Exchange: {ExchangeName}",
                retryCount,
                timeSpan.TotalSeconds,
                domainEvent.GetType().Name,
                domainEvent.CorrelationId,
                _settings.ExchangeName);
        });

        try
        {
            policy.Execute(() =>
                channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: "",
                    basicProperties: properties,
                    body: body
                )
            );
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex,
                "RabbitMQ is unreachable even after retries. EventType: {EventType}, CorrelationId: {CorrelationId}, Exchange: {ExchangeName}",
                domainEvent.GetType().Name,
                domainEvent.CorrelationId,
                _settings.ExchangeName);
        }
        catch (AlreadyClosedException ex)
        {
            _logger.LogError(ex,
                "RabbitMQ connection is closed. EventType: {EventType}, CorrelationId: {CorrelationId}, Exchange: {ExchangeName}",
                domainEvent.GetType().Name,
                domainEvent.CorrelationId,
                _settings.ExchangeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An unexpected error occurred when sending event to RabbitMQ. EventType: {EventType}, CorrelationId: {CorrelationId}, Exchange: {ExchangeName}",
                domainEvent.GetType().Name,
                domainEvent.CorrelationId,
                _settings.ExchangeName);
        }
        return Task.CompletedTask;
    }

}
