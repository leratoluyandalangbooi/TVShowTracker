using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.Abstractions.Services;

public interface IWatchedEpisodeService
{
    Task<IEnumerable<WatchedEpisodeDto>> GetUserWatchedEpisodesAsync(int userId, CancellationToken cancellationToken = default);
    Task MarkEpisodeAsWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task UnmarkEpisodeAsWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task UpdateWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task<bool> IsEpisodeWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
}
