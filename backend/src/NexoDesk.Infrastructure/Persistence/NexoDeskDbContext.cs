using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NexoDesk.Domain.Entities;
using NexoDesk.Infrastructure.Identity;

namespace NexoDesk.Infrastructure.Persistence;

public sealed class NexoDeskDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public NexoDeskDbContext(
        DbContextOptions<NexoDeskDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureSequences(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NexoDeskDbContext).Assembly);
    }

    private static void ConfigureSequences(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasSequence<int>("TicketNumberSequence", schema: "dbo")
            .StartsAt(1)
            .IncrementsBy(1);
    }
}