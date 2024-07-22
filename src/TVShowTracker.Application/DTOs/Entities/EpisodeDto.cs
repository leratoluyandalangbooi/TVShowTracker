namespace TVShowTracker.Application.DTOs.Entities;

public class EpisodeDto : BaseEntityDto
{
    public int TMDbEpisodeId { get; set; }
    public int SeasonId { get; set; }
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AirDate { get; set; }
    public int Runtime { get; set; }
    public string Overview { get; set; } = string.Empty;
    public string StillPath { get; set; } = string.Empty;

    public SeasonDto? Season { get; set; }
    public ICollection<WatchedEpisodeDto> WatchedEpisodes { get; set; } = new List<WatchedEpisodeDto>();
}
