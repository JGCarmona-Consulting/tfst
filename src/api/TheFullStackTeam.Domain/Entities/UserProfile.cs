using NUlid;
using System.Text.Json.Serialization;

namespace TheFullStackTeam.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Ulid AccountId { get; set; }
    [JsonIgnore]
    public Account? Account { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
