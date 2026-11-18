using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexoDesk.Domain.Entities;
using NexoDesk.Infrastructure.Identity;

namespace NexoDesk.Infrastructure.Persistence.Configurations;

public sealed class TicketHistoryConfiguration
    : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("TicketHistories");

        // Primary Key
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .ValueGeneratedNever();

        // TicketId
        builder.Property(h => h.TicketId)
            .IsRequired();

        // ActorId
        builder.Property(h => h.ActorId)
            .IsRequired();

        // EventType
        builder.Property(h => h.EventType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        // OldValue
        builder.Property(h => h.OldValue);

        // NewValue
        builder.Property(h => h.NewValue);

        // Description
        builder.Property(h => h.Description);

        // CreatedAtUtc
        builder.Property(h => h.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset(7)");

        // Relationship with Ticket
        builder.HasOne(h => h.Ticket)
            .WithMany(t => t.History)
            .HasForeignKey(h => h.TicketId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TicketHistories_Tickets_TicketId");

        // Actor relationship
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(h => h.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TicketHistories_AspNetUsers_ActorId");

        // Index
        builder.HasIndex(h => new { h.TicketId, h.CreatedAtUtc })
            .HasDatabaseName("IX_TicketHistories_TicketId_CreatedAtUtc");
    }
}