using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Blogs;

namespace Portfolio.Infrastructure.Configurations;

internal sealed class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.ToTable("blog_tags");

        builder.HasKey(bT => new { bT.BlogId, bT.TagId });
    }
}
