using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Infrastructure.Services;

public class WatchedEpisodeService : IWatchedEpisodeService
{
    private readonly IWatchedEpisodeRepository _watchedEpisodeRepository;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<WatchedEpisodeService> _logger;

    public WatchedEpisodeService(IWatchedEpisodeRepository watchedEpisodeRepository, IEpisodeRepository episodeRepository, IMapper mapper, ILogger<WatchedEpisodeService> logger)
    {
        _watchedEpisodeRepository = watchedEpisodeRepository ?? throw new ArgumentNullException(nameof(watchedEpisodeRepository));
        _episodeRepository = episodeRepository ?? throw new ArgumentNullException(nameof(episodeRepository));
        _mapper = mapper;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<WatchedEpisodeDto>> GetUserWatchedEpisodesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var watchedEpisodes = await _watchedEpisodeRepository.GetUserWatchedEpisodesAsync(userId, cancellationToken);
            var mapEpisodes = _mapper.Map<IEnumerable<WatchedEpisodeDto>>(watchedEpisodes);
            _logger.LogInformation($"Retrieved watched episodes for user {userId} with {mapEpisodes.Count()} items");
            return mapEpisodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting watched episodes for user {userId}");
            throw;
        }
    }

    public async Task MarkEpisodeAsWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _watchedEpisodeRepository.IsEpisodeWatchedAsync(userId, episodeId, cancellationToken))
            {
                _logger.LogWarning($"Episode {episodeId} is already marked as watched for user {userId}");
                throw new InvalidOperationException("Episode is already marked as watched.");
            }

            var dateNow = DateTime.UtcNow;
            var watchedEpisode = new WatchedEpisode
            {
                UserId = userId,
                EpisodeId = episodeId,
                WatchedDate = dateNow,
                CreatedAt = dateNow
            };

            await _watchedEpisodeRepository.AddWatchedEpisodeAsync(watchedEpisode, cancellationToken);
            _logger.LogInformation($"Marked episode {episodeId} as watched for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while marking episode {episodeId} as watched for user {userId}");
            throw;
        }
    }

    public async Task UnmarkEpisodeAsWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _watchedEpisodeRepository.RemoveWatchedEpisodeAsync(userId, episodeId, cancellationToken);
            _logger.LogInformation($"Unmarked episode {episodeId} as watched for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while unmarking episode {episodeId} as watched for user {userId}");
            throw;
        }
    }

    public async Task UpdateWatchedEpisodeAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var watchedEpisode = await _watchedEpisodeRepository.GetWatchedEpisodeAsync(userId, episodeId, cancellationToken);
            if (watchedEpisode == null)
            {
                _logger.LogWarning($"Attempted to update non-existent watched episode for user {userId} and episode {episodeId}");
                throw new KeyNotFoundException("Watched episode not found.");
            }

            var dateNow = DateTime.UtcNow;
            watchedEpisode.WatchedDate = dateNow;
            watchedEpisode.UpdatedAt = dateNow;  
            await _watchedEpisodeRepository.UpdateWatchedEpisodeAsync(watchedEpisode, cancellationToken);
            _logger.LogInformation($"Updated watched date for episode {episodeId} for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating watched date for episode {episodeId} for user {userId}");
            throw;
        }
    }

    public async Task<bool> IsEpisodeWatchedAsync(int userId, int episodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var isWatched = await _watchedEpisodeRepository.IsEpisodeWatchedAsync(userId, episodeId, cancellationToken);
            _logger.LogInformation($"Checked if episode {episodeId} is watched by user {userId}. Result: {isWatched}");
            return isWatched;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking if episode {episodeId} is watched by user {userId}");
            throw;
        }
    }
}