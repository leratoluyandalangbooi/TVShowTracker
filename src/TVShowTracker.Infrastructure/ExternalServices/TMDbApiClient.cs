namespace TVShowTracker.Infrastructure.ExternalServices;

public class TMDbApiClient : ITMDbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _baseUrl;
    private readonly ILogger<TMDbApiClient> _logger;
    private readonly IMemoryCache _cache;

    public TMDbApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<TMDbApiClient> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _apiKey = configuration["TMDb:ApiKey"];
        _baseUrl = configuration["TMDb:BaseUrl"];

        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("TMDb API configuration is missing or invalid.");
        }

        _httpClient.BaseAddress = new Uri(_baseUrl);
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<Show>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20)
    {
        string cacheKey = $"TopShows_{language}_{page}_{pageSize}";
        if (_cache.TryGetValue(cacheKey, out List<Show>? cachedShows))
        {
            _logger.LogInformation("Returning top shows from cache");
            return cachedShows;
        }

        try
        {
            _logger.LogInformation("Fetching top shows from TMDb API");
            
            var response = await _httpClient.GetAsync($"tv/popular?api_key={_apiKey}&language={language}&page={page}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TMDbShowResponse>(content);

            var topShows = result?.Results.Select(MapToShow).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, topShows, cacheEntryOptions);

            _logger.LogInformation($"Successfully fetched and cached {topShows?.Count} top shows");

            return topShows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching top shows");
            throw;
        }
    }

    public async Task<Show?> GetShowDetailsAsync(int showId)
    {
        string cacheKey = $"ShowDetails_{showId}";
        if (_cache.TryGetValue(cacheKey, out Show? cachedShowDetails))
        {
            _logger.LogInformation("Returning show details from cache");
            return cachedShowDetails;
        }

        try
        {
            _logger.LogInformation("Fetching show details from TMDb API");

            var response = await _httpClient.GetAsync($"tv/{showId}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TMDbShowDetail>(content);

            var showDetails = MapToShowDetail(result);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, showDetails, cacheEntryOptions);

            _logger.LogInformation($"Successfully fetched and cached show details");

            return showDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching show details");
            throw;
        }
        
    }

    public async Task<Episode?> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber)
    {
        string cacheKey = $"EpoisodeDetails_{showId}_{seasonNumber}_{episodeNumber}";
        if (_cache.TryGetValue(cacheKey, out Episode? cachedEpisodeDetails))
        {
            _logger.LogInformation("Returning episode from cache");
            return cachedEpisodeDetails;
        }

        try
        {
            _logger.LogInformation("Fetching episode details from TMDb API");

            var response = await _httpClient.GetAsync($"tv/{showId}/season/{seasonNumber}/episode/{episodeNumber}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TMDbEpisode>(content);

            // Fetch show details to properly map the episode
            var showResponse = await _httpClient.GetAsync($"tv/{showId}?api_key={_apiKey}");
            showResponse.EnsureSuccessStatusCode();
            var showContent = await showResponse.Content.ReadAsStringAsync();
            var showResult = JsonSerializer.Deserialize<TMDbShow>(showContent);

            var show = MapToShow(showResult);

            var episodeDetails = MapToEpisode(result, show);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, episodeDetails, cacheEntryOptions);

            _logger.LogInformation($"Successfully fetched and cached episode details");

            return episodeDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching episode details");
            throw;
        }

    }

    public async Task<List<Show>?> SearchShowsAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"search/tv?api_key={_apiKey}&query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<TMDbSearchResponse>(content);

            _logger.LogInformation("Successfully searched for shows with query: {Query}", query);

            return searchResult?.Results.Select(MapToShow).ToList();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while searching for shows with query: {Query}", query);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing TMDb API response for search query: {Query}", query);
            throw;
        }
    }

    private Show MapToShow(TMDbShow show)
    {
        return new Show
        {
            ShowId = show.Id,
            Name = show.Name,
            Overview = show.Overview,
            FirstAirDate = DateTime.Parse(show.FirstAirDate),
            Popularity = show.Popularity,
            PosterPath = show.PosterPath,
            Episodes = new List<Episode>()
        };
    }

    private Show MapToShowDetail(TMDbShowDetail show)
    {
        var mappedShow = MapToShow(show);

        mappedShow.Episodes = show.Seasons
            .SelectMany(s => s.Episodes.Select(e => MapToEpisode(e, mappedShow)))
            .ToList();

        return mappedShow;
    }

    private Episode MapToEpisode(TMDbEpisode episode, Show show)
    {
        return new Episode
        {
            Id = episode.Id,
            ShowId = episode.ShowId,
            SeasonNumber = episode.SeasonNumber,
            EpisodeNumber = episode.EpisodeNumber,
            Name = episode.Name,
            AirDate = DateTime.Parse(episode.AirDate),
            Show = show
        };
    }
}
