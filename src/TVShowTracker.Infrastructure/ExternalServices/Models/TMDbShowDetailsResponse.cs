namespace TVShowTracker.Infrastructure.ExternalServices.Models;

public class TMDbShowDetailsResponse : TMDbShowResponse
{

    [JsonPropertyName("seasons")]
    public List<TMDbSeasonResponse> Seasons { get; set; } = new List<TMDbSeasonResponse>();

    [JsonPropertyName("last_episode_to_air")]
    public TMDbEpisodeResponse? LastEpisodeToAir { get; set; }

    [JsonPropertyName("next_episode_to_air")]
    public TMDbEpisodeResponse? NextEpisodeToAir { get; set; }

    [JsonPropertyName("number_of_episodes")]
    public int NumberOfEpisodes { get; set; }

    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}
