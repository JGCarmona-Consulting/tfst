using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Application.UserProfiles.Models;

public class UserProfileModel
{
    public string? Id { get; set; }
    public string? AccountId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    public static UserProfileModel FromEntity(UserProfile userProfile)
    {
        return new UserProfileModel
        {
            Id = userProfile.Id.ToString(),
            AccountId = userProfile.AccountId.ToString(),
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            DisplayName = userProfile.DisplayName,
            DateOfBirth = userProfile.DateOfBirth,
            ProfilePictureUrl = userProfile.ProfilePictureUrl,
            PhoneNumber = userProfile.PhoneNumber,
            Gender = userProfile.Gender
        };
    }

    public static UserProfileModel FromView(UserProfileView view)
    {
        var model = new UserProfileModel
        {
            Id = view.EntityId,
            AccountId = view.AccountId,
            FirstName = view.FirstName,
            LastName = view.LastName,
            DisplayName = view.DisplayName,
            DateOfBirth = view.DateOfBirth,
            ProfilePictureUrl = view.ProfilePictureUrl,
            PhoneNumber = view.PhoneNumber,
            Gender = view.Gender,
        };

        return model;
    }
}
