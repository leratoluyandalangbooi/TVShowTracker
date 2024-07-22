namespace TVShowTracker.Application.DTOs.Entities;

public class BaseEntityDto
{
    public int Id { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
