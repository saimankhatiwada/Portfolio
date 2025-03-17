using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Infrastructure.Outbox;

namespace Portfolio.Infrastructure.Configurations;

/// <summary>
/// Provides the configuration for the <see cref="Portfolio.Infrastructure.Outbox.OutboxMessage"/> entity.
/// </summary>
/// <remarks>
/// This configuration maps the <see cref="Portfolio.Infrastructure.Outbox.OutboxMessage"/> entity to the "outbox_messages" table
/// and specifies the key and property configurations.
/// </remarks>
internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    /// <summary>
    /// Configures the <see cref="Portfolio.Infrastructure.Outbox.OutboxMessage"/> entity.
    /// </summary>
    /// <param name="builder">
    /// An <see cref="EntityTypeBuilder{TEntity}"/> used to configure the <see cref="Portfolio.Infrastructure.Outbox.OutboxMessage"/> entity.
    /// </param>
    /// <remarks>
    /// This method maps the <see cref="Portfolio.Infrastructure.Outbox.OutboxMessage"/> entity to the "outbox_messages" table,
    /// sets the primary key, and configures the "Content" property to use the "jsonb" column type.
    /// </remarks>
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Content).HasColumnType("jsonb");
    }
}
