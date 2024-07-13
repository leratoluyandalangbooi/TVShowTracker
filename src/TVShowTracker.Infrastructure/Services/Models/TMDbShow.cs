
namespace TVShowTracker.Infrastructure.Services.Models;

internal class TMDbShow
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = string.Empty;

    [JsonPropertyName("first_air_date")]
    public DateTime FirstAirDate { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
   
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = new List<int>();

    [JsonPropertyName("origin_country")]
    public List<string> OriginCountry { get; set; } = new List<string>();

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

}
