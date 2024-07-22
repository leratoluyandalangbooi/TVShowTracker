using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.DTOs.Responses;

public class TMDbTopShowsResponseDto
{
    public List<TVShowDto> Results { get; set; } = new List<TVShowDto>();
}