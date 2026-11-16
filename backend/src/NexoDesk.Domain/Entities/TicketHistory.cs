using NexoDesk.Domain.Enums;

namespace NexoDesk.Domain.Entities;

public sealed class TicketHistory
{
    private TicketHistory()
    {
    }

    public TicketHistory(
        Guid ticketId,
        Guid actorId,
        TicketHistoryType eventType,
        string? oldValue,
        string? newValue,
        string? description,
        DateTimeOffset createdAtUtc)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid ticket is required.",
                nameof(ticketId));
        }

        if (actorId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid actor is required.",
                nameof(actorId));
        }

        if (!Enum.IsDefined(eventType))
        {
            throw new ArgumentOutOfRangeException(nameof(eventType));
        }

        Id = Guid.NewGuid();

        TicketId = ticketId;
        ActorId = actorId;
        EventType = eventType;

        OldValue = oldValue;
        NewValue = newValue;
        Description = description;

        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; private set; }

    public Guid TicketId { get; private set; }

    public Guid ActorId { get; private set; }

    public TicketHistoryType EventType { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Ticket Ticket { get; private set; } = null!;
}