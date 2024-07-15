namespace TVShowTracker.Infrastructure.Services.Models;

public class TMDbShowDetail : TMDbShow
{
    [JsonPropertyName("seasons")]
    public List<TMDbSeason> Seasons { get; set; } = new List<TMDbSeason>();

    [JsonPropertyName("last_episode_to_air")]
    public TMDbEpisode? LastEpisodeToAir { get; set; }

    [JsonPropertyName("next_episode_to_air")]
    public TMDbEpisode? NextEpisodeToAir { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
