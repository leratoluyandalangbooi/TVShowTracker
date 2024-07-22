namespace TVShowTracker.Application.Abstractions.Repositories;

public interface IWatchedEpisodeRepository
{
    Task<IEnumerable<WatchedEpisode>> GetUserWatchedEpisodesAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsEpisodeWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task AddWatchedEpisodeAsync(WatchedEpisode watchedEpisode, CancellationToken cancellationToken = default);
    Task RemoveWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task<WatchedEpisode> GetWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default);
    Task UpdateWatchedEpisodeAsync(WatchedEpisode watchedEpisode, CancellationToken cancellationToken = default);
}
