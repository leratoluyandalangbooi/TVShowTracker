

using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.Abstractions;

public interface ITMDbApiClient
{
    Task<IEnumerable<TVShowDto>> GetTopShowsAsync(string language, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<TVShowDto> GetShowDetailsAsync(int showId, CancellationToken cancellationToken = default);
    Task<SeasonDto> GetShowSeasonDetailsAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default);
    Task<EpisodeDto> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<TVShowDto>> SearchShowsAsync(string query, CancellationToken cancellationToken = default);
}
