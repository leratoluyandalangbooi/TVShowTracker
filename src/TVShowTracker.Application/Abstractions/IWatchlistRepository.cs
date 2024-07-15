namespace TVShowTracker.Application.Abstractions;

public interface IWatchlistRepository
{
    Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId);
    Task<Watchlist?> GetWatchlistItemAsync(int userId, int showId);
    Task AddWatchlistItemAsync(Watchlist item);
    Task UpdateWatchlistItemAsync(Watchlist item);
    Task RemoveWatchlistItemAsync(int userId, int showId);
    Task<bool> WatchlistItemExistsAsync(int userId, int showId);
}
