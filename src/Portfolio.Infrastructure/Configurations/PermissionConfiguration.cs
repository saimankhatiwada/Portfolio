using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.HasData(
            Permission.UsersReadSelf,
            Permission.UsersRead,
            Permission.UsersReadSingle,
            Permission.UsersUpdate,
            Permission.UsersDelete,
            Permission.TagsRead,
            Permission.TagsReadSingle,
            Permission.TagsAdd,
            Permission.TagsUpdate,
            Permission.TagsDelete,
            Permission.BlogsRead,
            Permission.BlogsReadSingle,
            Permission.BlogsAdd,
            Permission.BlogsUpdate,
            Permission.BlogsDelete);
    }
}
