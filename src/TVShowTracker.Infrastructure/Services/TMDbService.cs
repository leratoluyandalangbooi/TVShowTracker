namespace TVShowTracker.Infrastructure.Services;

public class TMDbService : ITMDbService
{
    private readonly ITMDbApiClient _tmdbApiClient;
    private readonly IShowRepository _showsRepository;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISearchService _searchService;
    private readonly ILogger<TMDbService> _logger;
    private readonly IDistributedCache _cache;

    public TMDbService(ITMDbApiClient tmdbApiClient, IShowRepository showsRepository, IEpisodeRepository episodeRepository, ISearchService searchService, ILogger<TMDbService> logger, IDistributedCache cache)
    {
        _tmdbApiClient = tmdbApiClient;
        _showsRepository = showsRepository;
        _episodeRepository = episodeRepository;
        _searchService = searchService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<Show>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20)
    {
        string cacheKey = $"TopShows_{language}_{page}_{pageSize}";
        var cachedShows = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedShows))
        {
            _logger.LogInformation("Returning top shows from distributed cache");
            return JsonSerializer.Deserialize<List<Show>>(cachedShows);
        }

        try
        {
            _logger.LogInformation("Fetching top shows from API");
            var topShows = await _tmdbApiClient.GetTopShowsAsync(language, page, pageSize);

            _logger.LogInformation($"Saving {topShows?.Count()} shows to local database");

            if (topShows == null || !topShows.Any())
            {
                _logger.LogError("Failed to fetch top shows");
                throw new Exception("Failed to fetch top shows");
            }

            //save to DB
            await _showsRepository.AddRangeAsync(topShows);

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(topShows), cacheEntryOptions);

            return topShows;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching top shows from TMDb API, falling back to local database");
            //fallback to DB
            return await _showsRepository.GetTopShowsAsync(pageSize);
        }
    }

    public async Task<Show?> GetShowDetailsAsync(int showId)
    {
        string cacheKey = $"ShowDetails_{showId}";
        var cachedShow = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedShow))
        {
            _logger.LogInformation($"Returning show details for ID {showId} from distributed cache");
            return JsonSerializer.Deserialize<Show>(cachedShow);
        }

        try
        {
            _logger.LogInformation($"Fetching show details for ID {showId} from API");
            var show = await _tmdbApiClient.GetShowDetailsAsync(showId);

            _logger.LogInformation($"Saving show details for ID {showId} to local database");

            if (show == null)
            {
                _logger.LogError("Failed to fetch show details");
                throw new Exception("Failed to fetch show details");
            }

            //save to DB
            await _showsRepository.AddOrUpdateAsync(show);

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(show), cacheEntryOptions);

            return show;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching TV show details for ID {showId} from API, falling back to local database");
            //fallback to DB
            return await _showsRepository.GetShowByIdAsync(showId);
        }
    }

    public async Task<Episode?> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber)
    {
        string cacheKey = $"EpisodeDetails_{showId}_{seasonNumber}_{episodeNumber}";
        var cachedShow = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedShow))
        {
            _logger.LogInformation($"Returning episode details for IDs {cacheKey} from distributed cache");
            return JsonSerializer.Deserialize<Episode>(cachedShow);
        }

        try
        {
            _logger.LogInformation($"Fetching episode details for IDs {cacheKey} from API");
            var episode = await _tmdbApiClient.GetEpisodeDetailsAsync(showId, seasonNumber, episodeNumber);

            if (episode == null)
            {
                _logger.LogError("Failed to fetch episode details");
                throw new Exception("Failed to fetch episode details");
            }

            //save to DB
            await _episodeRepository.AddOrUpdateAsync(episode);

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(episode), cacheEntryOptions);

            return episode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching episode details for IDs {cacheKey} from API, falling back to local database");
            return await _episodeRepository.GetEpisodeAsync(showId, seasonNumber, episodeNumber);
        }
    }

    public async Task InvalidateShowCache(int showId)
    {
        string cacheKey = $"Show_{showId}";
        await _cache.RemoveAsync(cacheKey);
        _logger.LogInformation($"Invalidated cache for show ID {showId}");
    }

    public async Task<IEnumerable<Show>> SearchShowsAsync(string query)
    {
        try
        {
            _logger.LogInformation($"Searching for shows with query: {query}");
            var searchResults = await _searchService.SearchShowsAsync(query);

            if (!searchResults.Any())
            {
                _logger.LogInformation("No results found in Elastic Search, fetching from TMDb API");
                var apiResults = await _tmdbApiClient.SearchShowsAsync(query);

                //index API results for future searches
                await _searchService.IndexShowsAsync(apiResults);
                return apiResults;
            }

            return searchResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for shows");
            throw;
        }
    }

    public async Task IndexShowAsync(Show show)
    {
        try
        {
            await _searchService.IndexShowAsync(show);
            _logger.LogInformation($"Indexed show: {show.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while indexing show: {show.Name}");
            throw;
        }
    }
}
