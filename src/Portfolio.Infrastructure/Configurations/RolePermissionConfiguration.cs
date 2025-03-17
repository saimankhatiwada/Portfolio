using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

/// <summary>
/// Configures the entity type <see cref="RolePermission"/> for the database context.
/// </summary>
/// <remarks>
/// This configuration defines the table name, composite primary key, and seed data for the
/// <see cref="RolePermission"/> entity. It establishes the relationship between roles and permissions
/// within the system, ensuring proper access control.
/// </remarks>
internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    /// <summary>
    /// Configures the <see cref="RolePermission"/> entity for the database context.
    /// </summary>
    /// <param name="builder">The builder used to configure the <see cref="RolePermission"/> entity.</param>
    /// <remarks>
    /// This method sets up the table name, defines the composite primary key, and seeds initial data
    /// for the <see cref="RolePermission"/> entity. It ensures the proper mapping of roles and permissions
    /// within the database schema.
    /// </remarks>
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasData(
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UsersReadSelf.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.UsersReadSelf.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.UsersRead.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.UsersUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.UsersDelete.Id
            });
    }
}
