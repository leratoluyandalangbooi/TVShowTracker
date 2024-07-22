namespace TVShowTracker.Application.DTOs.Entities;

public class SeasonDto : BaseEntityDto
{
    public int TMDbSeasonId { get; set; }
    public int ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AirDate { get; set; }
    public int EpisodeCount { get; set; } = 0;
    public int SeasonNumber { get; set; }
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public ICollection<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
}
