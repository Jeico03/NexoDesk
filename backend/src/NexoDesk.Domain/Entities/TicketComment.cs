namespace NexoDesk.Domain.Entities;

public sealed class TicketComment
{
    private TicketComment()
    {
    }

    public TicketComment(
        Guid ticketId,
        Guid authorId,
        string content,
        DateTimeOffset createdAtUtc)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid ticket is required.",
                nameof(ticketId));
        }

        if (authorId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid author is required.",
                nameof(authorId));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException(
                "Comment content is required.",
                nameof(content));
        }

        content = content.Trim();

        if (content.Length > 2000)
        {
            throw new ArgumentException(
                "Comment cannot exceed 2000 characters.",
                nameof(content));
        }

        Id = Guid.NewGuid();

        TicketId = ticketId;
        AuthorId = authorId;
        Content = content;

        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; private set; }

    public Guid TicketId { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Ticket Ticket { get; private set; } = null!;
}