namespace TVShowTracker.Infrastructure.Repositories;

public class WatchedEpisodeRepository : IWatchedEpisodeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WatchedEpisodeRepository> _logger;

    public WatchedEpisodeRepository(ApplicationDbContext context, ILogger<WatchedEpisodeRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<WatchedEpisode>> GetUserWatchedEpisodesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.WatchedEpisodes
                .Where(w => w.UserId == userId)
                .Include(w => w.Episode)
                .ThenInclude(e => e.Season)
                .ThenInclude(s => s.TVShow)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving watched episodes for user {userId}");
            throw;
        }
    }

    public async Task<bool> IsEpisodeWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.WatchedEpisodes
                .AnyAsync(w => w.UserId == userId && w.EpisodeId == episodeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking if episode {episodeId} is watched by user {userId}");
            throw;
        }
    }

    public async Task AddWatchedEpisodeAsync(WatchedEpisode watchedEpisode, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.WatchedEpisodes.AddAsync(watchedEpisode, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding watched episode {watchedEpisode.EpisodeId} for user {watchedEpisode.UserId}");
            throw;
        }
    }

    public async Task RemoveWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var watchedEpisode = await _context.WatchedEpisodes
                .FirstOrDefaultAsync(w => w.UserId == userId && w.EpisodeId == episodeId, cancellationToken);

            if (watchedEpisode != null)
            {
                _context.WatchedEpisodes.Remove(watchedEpisode);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while removing watched episode {episodeId} for user {userId}");
            throw;
        }
    }

    public async Task<WatchedEpisode> GetWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.WatchedEpisodes
                .FirstOrDefaultAsync(w => w.UserId == userId && w.EpisodeId == episodeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving watched episode {episodeId} for user {userId}");
            throw;
        }
    }

    public async Task UpdateWatchedEpisodeAsync(WatchedEpisode watchedEpisode, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.WatchedEpisodes.Update(watchedEpisode);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating watched episode {watchedEpisode.EpisodeId} for user {watchedEpisode.UserId}");
            throw;
        }
    }
}