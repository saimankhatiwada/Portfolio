using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasMaxLength(100)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .HasConversion(firstName => firstName.Value, value => new FirstName(value));

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .HasConversion(firstName => firstName.Value, value => new LastName(value));

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .HasConversion(email => email.Value, value => new Email(value));

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(u => u.IdentityId).IsUnique();
    }
}
