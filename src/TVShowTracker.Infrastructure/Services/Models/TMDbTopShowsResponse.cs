namespace TVShowTracker.Infrastructure.Services.Models;

internal class TMDbTopShowsResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TMDbShow> Results { get; set; } = new List<TMDbShow>();

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
    
}
