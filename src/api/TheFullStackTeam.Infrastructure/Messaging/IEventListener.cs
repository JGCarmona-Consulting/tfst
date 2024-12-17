using TheFullStackTeam.Domain.Events;

namespace TheFullStackTeam.Infrastructure.Messaging;

public interface IEventListener
{
    Task StartListeningAsync(Func<EventBase, Task> onEventReceived, CancellationToken cancellationToken);
}
