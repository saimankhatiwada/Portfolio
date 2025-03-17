using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="Role"/> entity.
/// </summary>
/// <remarks>
/// This configuration maps the <see cref="Role"/> entity to the "roles" table in the database.
/// It defines the primary key, relationships with other entities, and seeds predefined roles such as
/// <see cref="Role.Registered"/> and <see cref="Role.SuperAdmin"/>.
/// </remarks>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <summary>
    /// Configures the <see cref="Role"/> entity for the Entity Framework Core model.
    /// </summary>
    /// <param name="builder">
    /// An <see cref="EntityTypeBuilder{Role}"/> instance used to configure the <see cref="Role"/> entity.
    /// </param>
    /// <remarks>
    /// This method maps the <see cref="Role"/> entity to the "roles" table, defines its primary key,
    /// establishes many-to-many relationships with <see cref="Permission"/> entities via <see cref="RolePermission"/>,
    /// and seeds predefined roles such as <see cref="Role.Registered"/> and <see cref="Role.SuperAdmin"/>.
    /// </remarks>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder.HasData(
            Role.Registered,
            Role.SuperAdmin);
    }
}
