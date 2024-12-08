using TheFullStackTeam.Domain.Events;

namespace TheFullStackTeam.Domain.Services;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : EventBase;
}
