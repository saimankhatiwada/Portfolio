using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Blogs;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.ToTable("blogs");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasMaxLength(100)
            .HasConversion(id => id.Value, value => new BlogId(value));

        builder.Property(b => b.Tittle)
            .HasMaxLength(50)
            .HasConversion(tittle => tittle.Value, value => new Tittle(value));

        builder.Property(b => b.Content)
            .HasMaxLength(30000)
            .HasConversion(content => content.Value, value => new Content(value));

        builder.Property(b => b.Summary)
            .HasMaxLength(1000)
            .HasConversion(summary => summary.Value, value => new Summary(value));

        builder.HasMany(b => b.Tags)
            .WithMany()
            .UsingEntity<BlogTag>();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId);
    }
}
