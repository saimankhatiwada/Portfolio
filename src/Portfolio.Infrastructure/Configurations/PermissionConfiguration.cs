using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="Permission"/> entity.
/// </summary>
/// <remarks>
/// This configuration class defines the table name, primary key, and seed data
/// for the <see cref="Permission"/> entity. It ensures that the entity is
/// properly mapped to the database and includes predefined permissions such as
/// <see cref="Permission.UsersReadSelf"/>, <see cref="Permission.UsersRead"/>,
/// <see cref="Permission.UsersUpdate"/>, and <see cref="Permission.UsersDelete"/>.
/// </remarks>
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <summary>
    /// Configures the <see cref="Permission"/> entity for the database context.
    /// </summary>
    /// <param name="builder">
    /// Provides a fluent API to configure the properties, relationships, and seed data
    /// for the <see cref="Permission"/> entity.
    /// </param>
    /// <remarks>
    /// This method maps the <see cref="Permission"/> entity to the "permissions" table,
    /// sets its primary key, and seeds predefined permissions such as
    /// <see cref="Permission.UsersReadSelf"/>, <see cref="Permission.UsersRead"/>,
    /// <see cref="Permission.UsersUpdate"/>, and <see cref="Permission.UsersDelete"/>.
    /// </remarks>
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.HasData(
            Permission.UsersReadSelf,
            Permission.UsersRead,
            Permission.UsersReadSingle,
            Permission.UsersUpdate,
            Permission.UsersDelete);
    }
}
