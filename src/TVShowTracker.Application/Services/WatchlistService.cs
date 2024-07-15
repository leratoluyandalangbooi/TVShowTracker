namespace TVShowTracker.Application.Services;

public class WatchlistService : IWatchlistService
{
    private readonly IWatchlistRepository _watchlistRepository;
    private readonly IShowRepository _showRepository;
    private readonly ILogger<WatchlistService> _logger;

    public WatchlistService(IWatchlistRepository watchlistRepository, IShowRepository showRepository, ILogger<WatchlistService> logger)
    {
        _watchlistRepository = watchlistRepository ?? throw new ArgumentNullException(nameof(watchlistRepository));
        _showRepository = showRepository ?? throw new ArgumentNullException(nameof(showRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId)
    {
        try
        {
            var watchlist = await _watchlistRepository.GetUserWatchlistAsync(userId);
            _logger.LogInformation($"Retrieved watchlist for user {userId} with {watchlist.Count()} items");
            return watchlist;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting watchlist for user {userId}");
            throw;
        }
    }

    public async Task AddToWatchlistAsync(int userId, int showId)
    {
        try
        {
            if (await _watchlistRepository.WatchlistItemExistsAsync(userId, showId))
            {
                _logger.LogWarning($"Show {showId} is already in the watchlist for user {userId}");
                throw new InvalidOperationException("Show is already in the watchlist.");
            }

            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show == null)
            {
                _logger.LogWarning($"Attempted to add non-existent show {showId} to watchlist for user {userId}");
                throw new KeyNotFoundException("Show not found.");
            }

            var watchlistItem = new Watchlist
            {
                UserId = userId,
                ShowId = showId,
                AddedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _watchlistRepository.AddWatchlistItemAsync(watchlistItem);
            _logger.LogInformation($"Added show {showId} to watchlist for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding show {showId} to watchlist for user {userId}");
            throw;
        }
    }

    public async Task RemoveFromWatchlistAsync(int userId, int showId)
    {
        try
        {
            await _watchlistRepository.RemoveWatchlistItemAsync(userId, showId);
            _logger.LogInformation($"Removed show {showId} from watchlist for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while removing show {showId} from watchlist for user {userId}");
            throw;
        }
    }

    public async Task UpdateWatchedEpisodeAsync(int userId, int showId, int episodeId)
    {
        try
        {
            var watchlistItem = await _watchlistRepository.GetWatchlistItemAsync(userId, showId);
            if (watchlistItem == null)
            {
                _logger.LogWarning($"Attempted to update non-existent watchlist item for user {showId} and show {userId}");
                throw new KeyNotFoundException("Watchlist item not found.");
            }

            watchlistItem.EpisodeId = episodeId;
            watchlistItem.UpdatedAt = DateTime.UtcNow;  
            await _watchlistRepository.UpdateWatchlistItemAsync(watchlistItem);
            _logger.LogInformation($"Updated watched episode to {episodeId} for show {showId} in watchlist for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating watched episode for show {showId} in watchlist for user {userId}");
            throw;
        }
    }

    public async Task<bool> IsShowInWatchlistAsync(int userId, int showId)
    {
        try
        {
            var exists = await _watchlistRepository.WatchlistItemExistsAsync(userId, showId);
            _logger.LogInformation($"Checked if show {showId} is in watchlist for user {userId}. Result: {exists}");
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking if show {showId} is in watchlist for user {userId}");
            throw;
        }
    }
}
