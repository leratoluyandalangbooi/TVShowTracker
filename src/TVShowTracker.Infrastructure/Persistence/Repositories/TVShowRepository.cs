namespace TVShowTracker.Infrastructure.Repositories;

public class TVShowRepository : ITVShowRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TVShowRepository> _logger;

    public TVShowRepository(ApplicationDbContext context, ILogger<TVShowRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TVShow>> GetTopShowsAsync(int? pageSize = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TVShows.OrderByDescending(s => s.Popularity).Take(pageSize ?? 20)
                                 .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting top {pageSize} shows");
            throw;
        }
    }

    public async Task<TVShow?> GetShowByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TVShows.AsNoTracking()
                                 .FirstOrDefaultAsync(s => s.TMDbShowId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting show for id {id}");
            throw;
        }
    }

    public async Task AddOrUpdateAsync(TVShow show, CancellationToken cancellationToken = default)
    {
        if (show == null)
        {
            throw new ArgumentNullException(nameof(show));
        }

        try
        {
            var existingShow = await _context.TVShows
                                    .FirstOrDefaultAsync(s => s.TMDbShowId == show.TMDbShowId, cancellationToken);

            if (existingShow == null)
            {
                show.Id = 0;
                show.CreatedAt = DateTime.UtcNow;

                _context.TVShows.Add(show);
            }
            else
            {
                if (existingShow.TMDbShowId != show.TMDbShowId) 
                {
                    show.UpdatedAt = DateTime.UtcNow;
                    _context.Entry(existingShow).CurrentValues.SetValues(show);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding or updating show for id {show.TMDbShowId}");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<TVShow> shows, CancellationToken cancellationToken = default)
    {
        if (shows == null || !shows.Any())
        {
            throw new ArgumentNullException(nameof(shows));
        }

        try
        {
            foreach (var show in shows)
            {
                await AddOrUpdateAsync(show, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding or updating multiple shows");
            throw;
        }
    }

    public async Task<IEnumerable<TVShow>> SearchShowsAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query))
        {
            throw new ArgumentException("Search query cannot be null or empty", nameof(query));
        }

        try
        {
            return await _context.TVShows
                        .Where(x => EF.Functions.Like(x.Name, $"%{query}%"))
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while searching shows with query: {query}");
            throw;
        }
    }
}