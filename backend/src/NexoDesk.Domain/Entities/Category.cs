namespace NexoDesk.Domain.Entities;

public sealed class Category
{
    private Category()
    {
    }

    public Category(
        string name,
        string? description,
        DateTimeOffset createdAtUtc)
    {
        Id = Guid.NewGuid();

        Name = NormalizeName(name);
        Description = NormalizeDescription(description);

        IsActive = true;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public ICollection<Ticket> Tickets { get; private set; }
        = new List<Ticket>();

    public void UpdateDetails(
        string name,
        string? description,
        DateTimeOffset updatedAtUtc)
    {
        Name = NormalizeName(name);
        Description = NormalizeDescription(description);

        UpdatedAtUtc = updatedAtUtc.ToUniversalTime();
    }

    public void Deactivate(DateTimeOffset updatedAtUtc)
    {
        IsActive = false;
        UpdatedAtUtc = updatedAtUtc.ToUniversalTime();
    }

    public void Activate(DateTimeOffset updatedAtUtc)
    {
        IsActive = true;
        UpdatedAtUtc = updatedAtUtc.ToUniversalTime();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name is required.",
                nameof(name));
        }

        name = name.Trim();

        if (name.Length > 100)
        {
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.",
                nameof(name));
        }

        return name;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        description = description.Trim();

        if (description.Length > 500)
        {
            throw new ArgumentException(
                "Category description cannot exceed 500 characters.",
                nameof(description));
        }

        return description;
    }
}