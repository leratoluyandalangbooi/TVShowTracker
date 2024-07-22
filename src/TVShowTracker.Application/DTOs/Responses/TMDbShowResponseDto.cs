namespace TVShowTracker.Application.DTOs.Responses;

public class TMDbShowResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string FirstAirDate { get; set; } = string.Empty;
    public double Popularity { get; set; }
    public string PosterPath { get; set; } = string.Empty;
}
