namespace TVShowTracker.Infrastructure.Services.Models;

public class TMDbShow
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("first_air_date")]
    public string FirstAirDate { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;
}
