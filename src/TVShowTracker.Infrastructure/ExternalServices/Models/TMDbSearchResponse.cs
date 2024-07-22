namespace TVShowTracker.Infrastructure.ExternalServices.Models;

public class TMDbSearchResponse
{
    [JsonPropertyName("results")]
    public List<TMDbShowResponse> Results { get; set; } = new List<TMDbShowResponse>();
}
