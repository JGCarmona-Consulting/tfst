﻿using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using TheFullStackTeam.Domain.Events;
using TheFullStackTeam.Domain.Services;
using System.Text;
using System.Text.Json;

namespace TheFullStackTeam.Infrastructure.Messaging.AzureEventBus;

public class AzureEventHubDispatcher : IEventDispatcher
{
    private readonly EventHubProducerClient _producerClient;

    public AzureEventHubDispatcher(string connectionString, string eventHubName)
    {
        _producerClient = new EventHubProducerClient(connectionString, eventHubName);
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : EventBase
    {
        var eventData = new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(domainEvent)));
        await _producerClient.SendAsync(new[] { eventData });
    }
}
