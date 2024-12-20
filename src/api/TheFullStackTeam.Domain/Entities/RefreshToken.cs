using NUlid;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheFullStackTeam.Domain.Entities;
public class RefreshToken : BaseEntity
{
    public Ulid AccountId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime ExpireDate { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
}
