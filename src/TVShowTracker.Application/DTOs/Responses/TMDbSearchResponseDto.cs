using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.DTOs.Responses;

public class TMDbSearchResponseDto
{
    public List<TVShowDto> Results { get; set; } = new List<TVShowDto>();
}
