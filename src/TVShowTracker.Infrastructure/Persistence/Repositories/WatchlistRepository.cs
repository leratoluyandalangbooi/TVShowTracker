namespace TVShowTracker.Infrastructure.Persistence.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WatchlistRepository> _logger;

    public WatchlistRepository(ApplicationDbContext context, ILogger<WatchlistRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId)
    {
        try
        {
            return await _context.Watchlist
                .Where(w => w.UserId == userId)
                .Include(w => w.Show)
                .Include(w => w.Episode)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting watchlist for user {userId}");
            throw;
        }
    }

    public async Task<Watchlist?> GetWatchlistItemAsync(int userId, int showId)
    {
        try
        {
            return await _context.Watchlist
                .Include(w => w.Show)
                .Include(w => w.Episode)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ShowId == showId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting watchlist item for user {userId} and show {userId}");
            throw;
        }
    }

    public async Task AddWatchlistItemAsync(Watchlist item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        try
        {
            await _context.Watchlist.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding watchlist item for user {item.UserId} and show {item.ShowId}");
            throw;
        }
    }

    public async Task UpdateWatchlistItemAsync(Watchlist item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        try
        {
            _context.Watchlist.Update(item);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating watchlist item for user {item.UserId} and show {item.ShowId}");
            throw;
        }
    }

    public async Task RemoveWatchlistItemAsync(int userId, int showId)
    {
        try
        {
            var item = await _context.Watchlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ShowId == showId);

            if (item != null)
            {
                _context.Watchlist.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while removing watchlist item for user {userId} and show {showId}");
            throw;
        }
    }

    public async Task<bool> WatchlistItemExistsAsync(int userId, int showId)
    {
        try
        {
            return await _context.Watchlist
                .AnyAsync(w => w.UserId == userId && w.ShowId == showId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking existence of watchlist item for user {userId} and show {showId}");
            throw;
        }
    }
}
