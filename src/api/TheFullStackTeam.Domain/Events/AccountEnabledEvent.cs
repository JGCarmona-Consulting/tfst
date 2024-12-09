namespace TheFullStackTeam.Domain.Events;

public class AccountEnabledEvent : EventBase
{
    public string AccountId { get; }

    public AccountEnabledEvent(string accountId)
    {
        AccountId = accountId;
    }
}
