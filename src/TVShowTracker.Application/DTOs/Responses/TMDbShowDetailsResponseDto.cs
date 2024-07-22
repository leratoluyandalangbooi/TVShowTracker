using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.DTOs.Responses;

public class TMDbShowDetailsResponseDto : TMDbShowResponseDto
{
    public List<SeasonDto> Seasons { get; set; } = new List<SeasonDto>();
    public EpisodeDto? LastEpisodeToAir { get; set; }
    public EpisodeDto? NextEpisodeToAir { get; set; }
    public int NumberOfEpisodes { get; set; }
    public int NumberOfSeasons { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}
    