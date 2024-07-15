namespace TVShowTracker.Infrastructure.Services.Models;

public class TMDbSearchResponse
{
    [JsonPropertyName("results")]
    public List<TMDbShow> Results { get; set; } = new List<TMDbShow>();
}
