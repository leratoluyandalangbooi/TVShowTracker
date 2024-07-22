namespace TVShowTracker.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}