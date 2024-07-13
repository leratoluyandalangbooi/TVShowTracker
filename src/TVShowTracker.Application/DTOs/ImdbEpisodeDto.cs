namespace TVShowTracker.Application.DTOs;

public class ImdbEpisodeDto
{
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string? Title { get; set; }
    public DateTime? AirDate { get; set; }
}
