using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexoDesk.Domain.Entities;
using NexoDesk.Infrastructure.Identity;

namespace NexoDesk.Infrastructure.Persistence.Configurations;

public sealed class TicketCommentConfiguration
    : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("TicketComments");

        // Primary Key
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        // TicketId
        builder.Property(c => c.TicketId)
            .IsRequired();

        // AuthorId
        builder.Property(c => c.AuthorId)
            .IsRequired();

        // Content
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);

        // CreatedAtUtc
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset(7)");

        // Relationship with Ticket
        builder.HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TicketComments_Tickets_TicketId");

        // Author relationship
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TicketComments_AspNetUsers_AuthorId");

        // Index
        builder.HasIndex(c => new { c.TicketId, c.CreatedAtUtc })
            .HasDatabaseName("IX_TicketComments_TicketId_CreatedAtUtc");
    }
}