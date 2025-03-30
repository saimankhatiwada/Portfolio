using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
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
                RoleId = Role.Registered.Id,
                PermissionId = Permission.BlogsRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.BlogsReadSingle.Id
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
                PermissionId = Permission.UsersReadSingle.Id
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
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.TagsRead.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.TagsReadSingle.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.TagsAdd.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.TagsUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.TagsDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.BlogsRead.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.BlogsReadSingle.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.BlogsAdd.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.BlogsUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.SuperAdmin.Id,
                PermissionId = Permission.BlogsDelete.Id
            });
    }
}
