using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoDesk.Domain.Entities;
using NexoDesk.Domain.Enums;

namespace NexoDesk.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration
    : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        // Primary Key
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        // Ticket Number
        builder.Property(t => t.Number)
            .HasColumnType("int")
            .HasDefaultValueSql(
                "NEXT VALUE FOR [dbo].[TicketNumberSequence]")
            .ValueGeneratedOnAdd();

        builder.HasIndex(t => t.Number)
            .IsUnique()
            .HasDatabaseName("UX_Tickets_Number");

        // Title
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(120);

        // Description
        builder.Property(t => t.Description)
            .IsRequired();

        // Category
        builder.Property(t => t.CategoryId)
            .IsRequired();

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tickets_Categories_CategoryId");

        // Priority
        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired()
            .HasDefaultValue(TicketPriority.Medium);

        // Status
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasDefaultValue(TicketStatus.Open);

        // Requester
        builder.Property(t => t.RequesterId)
            .IsRequired();

        // Assigned Technician
        builder.Property(t => t.AssignedTechnicianId);

        // Resolution Summary
        builder.Property(t => t.ResolutionSummary);

        // Dates
        builder.Property(t => t.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset(7)");

        builder.Property(t => t.UpdatedAtUtc)
            .HasColumnType("datetimeoffset(7)");

        builder.Property(t => t.ResolvedAtUtc)
            .HasColumnType("datetimeoffset(7)");

        builder.Property(t => t.ClosedAtUtc)
            .HasColumnType("datetimeoffset(7)");

        // Concurrency Token
        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // Indexes for common queries
        builder.HasIndex(t => t.RequesterId)
            .HasDatabaseName("IX_Tickets_RequesterId");

        builder.HasIndex(t => t.AssignedTechnicianId)
            .HasDatabaseName("IX_Tickets_AssignedTechnicianId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tickets_Status");

        builder.HasIndex(t => t.CreatedAtUtc)
            .HasDatabaseName("IX_Tickets_CreatedAtUtc");
    }
}