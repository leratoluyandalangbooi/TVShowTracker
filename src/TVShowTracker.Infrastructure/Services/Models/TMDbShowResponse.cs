namespace TVShowTracker.Infrastructure.Services.Models;

public class TMDbShowResponse
{
    [JsonPropertyName("results")]
    public List<TMDbShow> Results { get; set; } = new List<TMDbShow>();
}