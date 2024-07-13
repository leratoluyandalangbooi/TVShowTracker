using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TVShowTracker.Application.DTOs;
using TVShowTracker.Infrastructure.Services.Models;

namespace TVShowTracker.Infrastructure.Services.Externals;

public class TMDbService : ITMDbService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _baseUrl;
    private readonly ILogger<TMDbService> _logger;
    private readonly IShowService _showService;
    private readonly IMemoryCache _memoryCache;

    public TMDbService(HttpClient httpClient, IConfiguration configuration, ILogger<TMDbService> logger, IShowService showService, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _apiKey = configuration["TMDb:ApiKey"] ?? string.Empty;
        _baseUrl = configuration["TMDb:BaseUrl"] ?? string.Empty;
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _logger = logger;
        _showService = showService;
        _memoryCache = memoryCache;
    }

    public async Task<List<TopShowDto>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20)
    {
        var cacheKey = $"TopShows_{language}_{page}_{pageSize}";
        if (!_memoryCache.TryGetValue(cacheKey, out List<TopShowDto>? topShows))
        {
            try
            {
                var response = await _httpClient.GetAsync($"tv/top_rated?api_key={_apiKey}&language{language}&page={page}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var tmdbResponse = JsonSerializer.Deserialize<TMDbTopShowsResponse>(content);

                var count = tmdbResponse?.Results.Count();
                var result = tmdbResponse?.Results;

                
                topShows = MapToTopShowDtos(tmdbResponse!).Take(pageSize).ToList();

                //save to DB
                await _showService.SaveTopShowsToDatabaseAsync(topShows);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _memoryCache.Set(cacheKey, topShows, cacheEntryOptions);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching top shows from TMDb API");
                //fallback to DB
                topShows = await _showService.GetTopShowsFromDatabase(page, pageSize);
            }
        }
        return topShows;
    }

    private List<TopShowDto> MapToTopShowDtos(TMDbTopShowsResponse topShows)
    {
        return topShows.Results.Select(s => new TopShowDto
        {
            Id = s.Id.ToString(),
            Name = s.Name,
            OriginalName = s.OriginalName,
            FirstAirDate = s.FirstAirDate,
            VoteAverage = s.VoteAverage,
            VoteCount = s.VoteCount,
            Adult = s.Adult,
            BackdropPath = s.BackdropPath,
            GenreIds = s.GenreIds,
            OriginCountry = s.OriginCountry,
            OriginalLanguage = s.OriginalLanguage,
            Overview = s.Overview,
            Popularity = s.Popularity,
            PosterPath = s.PosterPath,
        }).ToList();
    }
}
