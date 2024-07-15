namespace TVShowTracker.Infrastructure.Persistence.Repositories;

public class EpisodeRepository : IEpisodeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EpisodeRepository> _logger;

    public EpisodeRepository(ApplicationDbContext context, ILogger<EpisodeRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Episode?> GetEpisodeAsync(int showId, int seasonNumber, int episodeNumber)
    {
        try
        {
            return await _context.Episodes
            .Include(e => e.Show)
            .FirstOrDefaultAsync(e => e.ShowId == showId &&
                                      e.SeasonNumber == seasonNumber &&
                                      e.EpisodeNumber == episodeNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting episode for showId {showId}, seasonNumber {seasonNumber} and episodeNumber {episodeNumber}");
            throw;
        }
    }

    public async Task AddOrUpdateAsync(Episode episode)
    {
        if (episode == null)
        {
            throw new ArgumentNullException(nameof(episode));
        }

        try
        {
            var existingEpisode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.ShowId == episode.ShowId &&
                                      e.SeasonNumber == episode.SeasonNumber &&
                                      e.EpisodeNumber == episode.EpisodeNumber);

            if (existingEpisode == null)
            {
                episode.CreatedAt = DateTime.UtcNow;
                _context.Episodes.Add(episode);
            }
            else
            {
                if(existingEpisode.ShowId != episode.ShowId &&
                                      existingEpisode.SeasonNumber != episode.SeasonNumber &&
                                      existingEpisode.EpisodeNumber != episode.EpisodeNumber)
                {
                    episode.UpdatedAt = DateTime.UtcNow;
                    _context.Entry(existingEpisode).CurrentValues.SetValues(episode);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding or updating episode for showId {episode.ShowId}, seasonNumber {episode.SeasonNumber} and episodeNumber {episode.EpisodeNumber}");
            throw;
        }
    }
}
