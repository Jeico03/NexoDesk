using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoDesk.Domain.Entities;

namespace NexoDesk.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Primary Key
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        // Name
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Unique Name
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("UX_Categories_Name");

        // Description
        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // IsActive
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // CreatedAtUtc
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset(7)");

        // UpdatedAtUtc
        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnType("datetimeoffset(7)");
    }
}