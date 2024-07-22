namespace TVShowTracker.Application.DTOs.Entities;

public class TVShowDto : BaseEntityDto
{
    public int TMDbShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime FirstAirDate { get; set; }
    public double Popularity { get; set; }
    public string PosterPath { get; set; } = string.Empty;
    public ICollection<SeasonDto> Seasons { get; set; } = new List<SeasonDto>();

    public EpisodeDto? LastEpisodeToAir { get; set; }
    public EpisodeDto? NextEpisodeToAir { get; set; }
    public int NumberOfEpisodes { get; set; }
    public int NumberOfSeasons { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int VoteCount { get; set; }

}
