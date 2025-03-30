using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Tags;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasMaxLength(100)
            .HasConversion(id => id.Value, value => new TagId(value));

        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .HasConversion(name => name.Value, value => new Name(value));

        builder.Property(t => t.Description)
            .HasMaxLength(100)
            .HasConversion(description => description!.Value, value => new Description(value));

        builder.HasIndex(t => new { t.Name })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId);
    }
}
