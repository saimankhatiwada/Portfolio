using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="User"/> entity.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IEntityTypeConfiguration{TEntity}"/> interface to define
/// the entity framework configuration for the <see cref="User"/> domain entity. It specifies
/// table mappings, property constraints, and indexes for the <see cref="User"/> entity.
/// </remarks>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the <see cref="User"/> entity for the Entity Framework Core model.
    /// </summary>
    /// <param name="builder">
    /// An <see cref="EntityTypeBuilder{TEntity}"/> instance used to configure the <see cref="User"/> entity.
    /// </param>
    /// <remarks>
    /// This method defines the table name, primary key, property constraints, conversions, and indexes
    /// for the <see cref="User"/> entity in the database schema.
    /// </remarks>
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
