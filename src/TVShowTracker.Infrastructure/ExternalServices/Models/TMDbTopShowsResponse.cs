namespace TVShowTracker.Infrastructure.ExternalServices.Models;

public class TMDbTopShowsResponse
{
    [JsonPropertyName("results")]
    public List<TMDbShowResponse> Results { get; set; } = new List<TMDbShowResponse>();
}