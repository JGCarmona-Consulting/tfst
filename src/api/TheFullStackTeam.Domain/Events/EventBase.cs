using NUlid;

namespace TheFullStackTeam.Domain.Events;

public abstract class EventBase
{
    public Ulid Id { get; } = Ulid.NewUlid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public string CorrelationId { get; set; } = string.Empty;
}
