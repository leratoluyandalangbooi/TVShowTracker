namespace TVShowTracker.Application.Abstractions;

public interface IWatchlistService
{
    Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId);
    Task AddToWatchlistAsync(int userId, int showId);
    Task RemoveFromWatchlistAsync(int userId, int showId);
    Task UpdateWatchedEpisodeAsync(int userId, int showId, int episodeId);
    Task<bool> IsShowInWatchlistAsync(int userId, int showId);
}
