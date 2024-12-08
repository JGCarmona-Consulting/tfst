using TheFullStackTeam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.EntityConfigurations;

public class UserProfileEntityConfiguration : BaseEntityConfiguration<UserProfile>
{
    public override void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        base.Configure(builder);

        builder.Property(up => up.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(up => up.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(up => up.DisplayName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(up => up.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(up => up.Gender)
               .HasMaxLength(10);

        builder.Property(up => up.IsPrimary)
               .IsRequired();

        builder.HasIndex(up => up.AccountId)
               .IsUnique(false);

        builder.HasOne(up => up.Account)
               .WithMany(a => a.Profiles)
               .HasForeignKey(up => up.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
