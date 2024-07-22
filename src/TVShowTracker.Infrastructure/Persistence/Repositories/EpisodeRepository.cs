using TVShowTracker.Domain.Entities;

namespace TVShowTracker.Infrastructure.Repositories;

public class EpisodeRepository : IEpisodeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EpisodeRepository> _logger;

    public EpisodeRepository(ApplicationDbContext context, ILogger<EpisodeRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Episode?> GetEpisodeAsync(int showId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Episodes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ShowId == showId &&
                                          e.SeasonNumber == seasonNumber &&
                                          e.EpisodeNumber == episodeNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting episode for showId {showId}, seasonNumber {seasonNumber} and episodeNumber {episodeNumber}");
            throw;
        }
    }

    public async Task<IEnumerable<Episode>> GetEpisodesForSeasonAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Episodes
                .Where(e => e.ShowId == showId && e.SeasonNumber == seasonNumber)
                .OrderBy(e => e.EpisodeNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting episodes for showId {showId} and seasonNumber {seasonNumber}");
            throw;
        }
    }

    public async Task AddOrUpdateAsync(Episode episode, CancellationToken cancellationToken = default)
    {
        if (episode == null)
        {
            throw new ArgumentNullException(nameof(episode));
        }

        try
        {
            var existingEpisode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.TMDbEpisodeId == episode.TMDbEpisodeId && 
                                          e.ShowId == episode.ShowId &&
                                          e.SeasonId == episode.SeasonId &&
                                          e.SeasonNumber == episode.SeasonNumber &&
                                          e.EpisodeNumber == episode.EpisodeNumber,
                                     cancellationToken);

            if (existingEpisode == null)
            {
                if (episode.SeasonId != 0)
                {
                    episode.Id = 0;
                    episode.CreatedAt = DateTime.UtcNow;

                    _context.Episodes.Add(episode);
                }
            }
            else
            {
                if(existingEpisode.TMDbEpisodeId != episode.TMDbEpisodeId &&
                   existingEpisode.ShowId != episode.ShowId &&
                   existingEpisode.SeasonId != episode.SeasonId &&
                   existingEpisode.SeasonNumber != episode.SeasonNumber &&
                   existingEpisode.EpisodeNumber != episode.EpisodeNumber)
                {
                    episode.UpdatedAt = DateTime.UtcNow;
                    _context.Entry(existingEpisode).CurrentValues.SetValues(episode);
                }

            }

            await BeginDatabaseTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding or updating episode for showId {episode.ShowId}, seasonNumber {episode.SeasonNumber} and episodeNumber {episode.EpisodeNumber}");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Episode> episodes, CancellationToken cancellationToken = default)
    {
        if (episodes == null || !episodes.Any())
        {
            throw new ArgumentNullException(nameof(episodes));
        }

        try
        {
            foreach (var episode in episodes)
            {
                await AddOrUpdateAsync(episode, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding or updating multiple episodes");
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