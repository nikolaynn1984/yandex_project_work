using Bookings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.Infrastructure.DataAccess;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(b => b.OccurredOn)
            .HasColumnName("occured_on")
            .IsRequired();

        builder.Property(b => b.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(b => b.Body)
            .HasColumnName("body")
            .IsRequired();


        builder.Property(b => b.IsProcessed)
            .HasColumnName("is_progress")
            .IsRequired();
    }
}
