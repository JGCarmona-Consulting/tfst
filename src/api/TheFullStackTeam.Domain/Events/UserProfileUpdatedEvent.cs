using TheFullStackTeam.Domain.Entities;

namespace TheFullStackTeam.Domain.Events;

public class UserProfileUpdatedEvent : EventBase
{
    public UserProfile UserProfile { get; }

    public UserProfileUpdatedEvent(UserProfile userProfile)
    {
        UserProfile = userProfile;
    }
}
