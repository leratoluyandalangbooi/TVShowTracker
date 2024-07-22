namespace TVShowTracker.Application.DTOs.Responses;

public class TMDbEpisodeResponseDto
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AirDate { get; set; } = string.Empty;
    public string StillPath { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int? Runtime { get; set; }
}
