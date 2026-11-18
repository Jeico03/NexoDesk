using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexoDesk.Infrastructure.Identity;

namespace NexoDesk.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset(7)");

        builder.Property(u => u.UpdatedAtUtc)
            .HasColumnType("datetimeoffset(7)");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_AspNetUsers_IsActive");
    }
}