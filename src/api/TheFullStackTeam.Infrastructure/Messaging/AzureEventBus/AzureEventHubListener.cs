﻿using TheFullStackTeam.Domain.Events;

namespace TheFullStackTeam.Infrastructure.Messaging.AzureEventBus;
public class AzureEventHubListener : IEventListener
{
    public Task StartListeningAsync(Func<EventBase, Task> onEventReceived, CancellationToken cancellationToken)
    {
        // TODO: Implement the logic to listen to the Azure Event Hub
        throw new NotImplementedException();
    }
}
