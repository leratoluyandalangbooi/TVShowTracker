namespace TVShowTracker.Application.DTOs;

public class TopShowDto
{
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public DateTime FirstAirDate { get; set; }

    public double VoteAverage { get; set; }

    public int VoteCount { get; set; }

    public bool Adult { get; set; }

    public string BackdropPath { get; set; } = string.Empty;

    public List<int> GenreIds { get; set; } = new List<int>();

    public List<string> OriginCountry { get; set; } = new List<string>();

    public string OriginalLanguage { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public double Popularity { get; set; }

    public string PosterPath { get; set; } = string.Empty;
}
