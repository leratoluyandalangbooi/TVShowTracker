using AutoMapper;
using System.Text;
using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Infrastructure.Services
{
    public class TMDbService : ITMDbService
    {
        private readonly ITMDbApiClient _tmdbApiClient;
        private readonly ITVShowRepository _showsRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly ILogger<TMDbService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public TMDbService(
            ITMDbApiClient tmdbApiClient,
            ITVShowRepository showsRepository,
            ISeasonRepository seasonRepository,
            IEpisodeRepository episodeRepository,
            ILogger<TMDbService> logger,
            IDistributedCache cache,
            IMapper mapper)
        {
            _tmdbApiClient = tmdbApiClient ?? throw new ArgumentNullException(nameof(tmdbApiClient));
            _showsRepository = showsRepository ?? throw new ArgumentNullException(nameof(showsRepository));
            _seasonRepository = seasonRepository ?? throw new ArgumentNullException(nameof(seasonRepository));
            _episodeRepository = episodeRepository ?? throw new ArgumentNullException(nameof(episodeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TVShowDto>> GetTopShowsAsync(string language, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"TopShows_{language}_{page}_{pageSize}";
            byte[] cachedData = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedData != null)
            {
                _logger.LogInformation("Returning top shows from distributed cache");
                string cachedJson = Encoding.UTF8.GetString(cachedData);
                return JsonSerializer.Deserialize<IEnumerable<TVShowDto>>(cachedJson);
            }

            try
            {
                _logger.LogInformation("Fetching top shows from API");
                var topShows = await _tmdbApiClient.GetTopShowsAsync(language, page, pageSize, cancellationToken);

                if (topShows == null || !topShows.Any())
                {
                    _logger.LogError("Failed to fetch top shows");
                    throw new Exception("Failed to fetch top shows");
                }

                var shows = _mapper.Map<IEnumerable<TVShow>>(topShows);
                await _showsRepository.AddRangeAsync(shows, cancellationToken);

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(topShows), cacheEntryOptions, cancellationToken);

                return topShows;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching top shows from TMDb API, falling back to local database");
                var dbShows = await _showsRepository.GetTopShowsAsync(pageSize, cancellationToken);
                return _mapper.Map<IEnumerable<TVShowDto>>(dbShows);
            }
        }

        public async Task<TVShowDto> GetShowDetailsAsync(int showId, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"ShowDetails_{showId}";
            byte[] cachedShow = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedShow != null)
            {
                _logger.LogInformation($"Returning show details for ID {showId} from distributed cache");
                string cachedJson = Encoding.UTF8.GetString(cachedShow);
                return JsonSerializer.Deserialize<TVShowDto>(cachedJson);
            }

            try
            {
                _logger.LogInformation($"Fetching show details for ID {showId} from API");
                var show = await _tmdbApiClient.GetShowDetailsAsync(showId, cancellationToken);

                if (show == null)
                {
                    _logger.LogError("Failed to fetch show details");
                    throw new Exception("Failed to fetch show details");
                }

                var showEntity = _mapper.Map<TVShow>(show);
                await _showsRepository.AddOrUpdateAsync(showEntity, cancellationToken);

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(show), cacheEntryOptions, cancellationToken);

                return show;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching TV show details for ID {showId} from API, falling back to local database");
                var showData = await _showsRepository.GetShowByIdAsync(showId, cancellationToken);
                return _mapper.Map<TVShowDto>(showData);
            }
        }

        public async Task<SeasonDto> GetShowSeasonDetailsAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"ShowSeason_{showId}_{seasonNumber}";
            byte[] cachedSeasons = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedSeasons != null)
            {
                _logger.LogInformation($"Returning season details for show ID {showId}, season {seasonNumber} from distributed cache");
                string cachedJson = Encoding.UTF8.GetString(cachedSeasons);
                return JsonSerializer.Deserialize<SeasonDto>(cachedJson);
            }

            try
            {
                _logger.LogInformation($"Fetching season details for show ID {showId}, season {seasonNumber} from API");
                var season = await _tmdbApiClient.GetShowSeasonDetailsAsync(showId, seasonNumber, cancellationToken);

                if (season == null)
                {
                    _logger.LogError("Failed to fetch season details");
                    throw new Exception("Failed to fetch season details");
                }

                var seasonEntity = _mapper.Map<Season>(season);
                await _seasonRepository.AddOrUpdateAsync(seasonEntity, cancellationToken);

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(season), cacheEntryOptions, cancellationToken);

                return season;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching season details for show ID {showId} from API, falling back to local database");
                var seasonData = await _seasonRepository.GetSeasonsForShowAsync(showId, cancellationToken);
                return _mapper.Map<SeasonDto>(seasonData);
            }
        }

        public async Task<EpisodeDto> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"EpisodeDetails_{showId}_{seasonNumber}_{episodeNumber}";
            byte[] cachedEpisode = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedEpisode != null)
            {
                _logger.LogInformation($"Returning episode details for IDs {cacheKey} from distributed cache");
                string cachedJson = Encoding.UTF8.GetString(cachedEpisode);
                return JsonSerializer.Deserialize<EpisodeDto>(cachedJson);
            }

            try
            {
                _logger.LogInformation($"Fetching episode details for IDs {cacheKey} from API");
                var episode = await _tmdbApiClient.GetEpisodeDetailsAsync(showId, seasonNumber, episodeNumber, cancellationToken);

                if (episode == null)
                {
                    _logger.LogError("Failed to fetch episode details");
                    throw new Exception("Failed to fetch episode details");
                }

                var episodeEntity = _mapper.Map<Episode>(episode);
                await _episodeRepository.AddOrUpdateAsync(episodeEntity, cancellationToken);

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(episode), cacheEntryOptions, cancellationToken);

                return episode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching episode details for IDs {cacheKey} from API, falling back to local database");
                var episodeData = await _episodeRepository.GetEpisodeAsync(showId, seasonNumber, episodeNumber, cancellationToken);
                return _mapper.Map<EpisodeDto>(episodeData);
            }
        }

        public async Task<IEnumerable<TVShowDto>> SearchShowsAsync(string query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("No results found in Elastic Search, fetching from TMDb API");
                var apiResults = await _tmdbApiClient.SearchShowsAsync(query, cancellationToken);

                if (apiResults == null || !apiResults.Any())
                {
                    _logger.LogError("No results found in TMDb API Search, searching from database");
                    var dbResults = await _showsRepository.SearchShowsAsync(query, cancellationToken);
                    return _mapper.Map<IEnumerable<TVShowDto>>(dbResults);
                }

                return apiResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for shows, now searching local database");
                var dbResults = await _showsRepository.SearchShowsAsync(query, cancellationToken);
                return _mapper.Map<IEnumerable<TVShowDto>>(dbResults);
            }
        }

        public async Task InvalidateCacheAsync(string cacheKey, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogInformation($"Cache for {cacheKey} now removed");
            }
            _logger.LogInformation($"Cache for {cacheKey} not found");
        }
    }
}