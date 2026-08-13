using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventInfrastructure.DataAccess.Configurations;
/// <summary>
/// Конфигурация пользователей
/// </summary>
internal class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(t => t.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(b => b.Login)
            .HasColumnName("login")
            .IsRequired();

        builder.Property(b => b.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(b => b.Role)
            .HasColumnName("role")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(s => s.Login)
            .IsUnique();


    }
}
