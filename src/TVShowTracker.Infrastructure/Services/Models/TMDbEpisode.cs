namespace TVShowTracker.Infrastructure.Services.Models;

public class TMDbEpisode
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("show_id")]
    public int ShowId { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = string.Empty;

    [JsonPropertyName("still_path")]
    public string StillPath { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("runtime")]
    public int Runtime { get; set; }
}
