using NUlid;

namespace TheFullStackTeam.Domain.Events;

public class AccountVerifiedEvent : EventBase
{
    public Ulid AccountId { get; }

    public AccountVerifiedEvent(Ulid accountId)
    {
        AccountId = accountId;
    }
}
