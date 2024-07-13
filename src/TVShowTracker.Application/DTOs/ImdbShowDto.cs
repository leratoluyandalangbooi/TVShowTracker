namespace TVShowTracker.Application.DTOs;

public class ImdbShowDto
{
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public List<ImdbEpisodeDto>? Episodes { get; set; }
}
