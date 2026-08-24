using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.DataAccess.Configurations;

internal class InboxConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox");

        builder.HasKey(b => b.Id);

        builder.HasIndex(d => d.Id)
            .IsUnique();

        builder.Property(e => e.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

        builder.Property(e => e.ReceivedOn)
            .HasColumnName("received_on")
            .IsRequired();
    }
}
