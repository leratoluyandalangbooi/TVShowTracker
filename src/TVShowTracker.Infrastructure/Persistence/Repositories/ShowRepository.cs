namespace TVShowTracker.Infrastructure.Persistence.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShowRepository> _logger;

    public ShowRepository(ApplicationDbContext context, ILogger<ShowRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Show>> GetTopShowsAsync(int pageSize = 20)
    {
        try
        {
            return await _context.Shows
            .OrderByDescending(s => s.Popularity)
            .Take(pageSize)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting top shows");
            throw;
        }
    }

    public async Task<Show?> GetShowByIdAsync(int id)
    {
        try
        {
            return await _context.Shows
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.ShowId == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting show for id {id}");
            throw;
        }
    }

    public async Task AddOrUpdateAsync(Show show)
    {
        if (show == null)
        {
            throw new ArgumentNullException(nameof(show));
        }

        try
        {
            var existingShow = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == show.ShowId);

            if (existingShow == null)
            {
                show.CreatedAt = DateTime.Now;
                _context.Shows.Add(show);
            }
            else
            {
                if (existingShow.ShowId != show.ShowId)
                {
                    show.UpdatedAt = DateTime.Now;
                    _context.Entry(existingShow).CurrentValues.SetValues(show);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding or updating show for id {show.ShowId}");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Show> shows)
    {
        if (shows == null || !shows.Any())
        {
            throw new ArgumentNullException(nameof(shows));
        }

        foreach (var show in shows)
        {
            await AddOrUpdateAsync(show);
        }
    }
}