namespace TVShowTracker.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeasonRepository> _logger;

    public SeasonRepository(ApplicationDbContext context, ILogger<SeasonRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Season?> GetSeasonAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Seasons
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShowId == showId && s.SeasonNumber == seasonNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting season for showId {showId} and seasonNumber {seasonNumber}");
            throw;
        }
    }

    public async Task<IEnumerable<Season>> GetSeasonsForShowAsync(int showId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Seasons
                .Where(s => s.ShowId == showId)
                .OrderBy(s => s.SeasonNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting seasons for showId {showId}");
            throw;
        }
    }

    public async Task AddOrUpdateAsync(Season season, CancellationToken cancellationToken = default)
    {
        if (season == null)
        {
            throw new ArgumentNullException(nameof(season));
        }

        try
        {
            var existingSeason = await _context.Seasons
                .FirstOrDefaultAsync(s => s.TMDbSeasonId == season.TMDbSeasonId && 
                                          s.ShowId == season.ShowId && 
                                          s.SeasonNumber == season.SeasonNumber, cancellationToken);

            if (existingSeason == null)
            {
                if(season.ShowId != 0)
                {
                    season.Id = 0;
                    season.EpisodeCount = season.Episodes.Count;
                    season.CreatedAt = DateTime.UtcNow;

                    _context.Seasons.Add(season);
                }
            }
            else
            {
                if (existingSeason.TMDbSeasonId != season.TMDbSeasonId &&
                    existingSeason.ShowId != season.ShowId &&
                    existingSeason.SeasonNumber != season.SeasonNumber)
                {
                    season.EpisodeCount = season.Episodes.Count;
                    season.UpdatedAt = DateTime.UtcNow;

                    _context.Entry(existingSeason).CurrentValues.SetValues(season);
                }
            }

            await BeginDatabaseTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding or updating season for showId {season.ShowId} and seasonNumber {season.SeasonNumber}");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Season> seasons, CancellationToken cancellationToken = default)
    {
        if (seasons == null || !seasons.Any())
        {
            throw new ArgumentNullException(nameof(seasons));
        }

        try
        {
            foreach (var season in seasons)
            {
                await AddOrUpdateAsync(season, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding or updating multiple seasons");
            throw;
        }
    }

    private async Task BeginDatabaseTransactionAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}