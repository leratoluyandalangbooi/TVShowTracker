using AutoMapper;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using TVShowTracker.Application.DTOs.Entities;
using TVShowTracker.Infrastructure.ExternalServices.Models;
using TVShowTracker.Infrastructure.Settings;

namespace TVShowTracker.Infrastructure.ExternalServices
{
    public class TMDbApiClient : ITMDbApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TMDbOptions _options;
        private readonly ILogger<TMDbApiClient> _logger;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public TMDbApiClient(
            IHttpClientFactory httpClientFactory,
            IOptions<TMDbOptions> options,
            ILogger<TMDbApiClient> logger,
            IMemoryCache cache,
            IMapper mapper)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            // base client auth
            var client = _httpClientFactory.CreateClient("TMDbApi");
            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ReadAccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<TVShowDto>> GetTopShowsAsync(string language, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"TopShows_{language}_{page}_{pageSize}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<TVShowDto> cachedShows))
            {
                _logger.LogInformation("Returning top shows from cache");
                return cachedShows;
            }

            var queryParams = new Dictionary<string, string>
            {
                ["api_key"] = _options.ApiKey,
                ["language"] = language,
                ["page"] = page.ToString(),
                ["pagesize"] = pageSize.ToString()
            };

            var shows = await GetAsync<TMDbTopShowsResponse>("tv/popular", queryParams, cancellationToken);
            var mappedShows = _mapper.Map<IEnumerable<TVShowDto>>(shows.Results);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, mappedShows, cacheEntryOptions);

            return mappedShows;
        }

        public async Task<TVShowDto> GetShowDetailsAsync(int showId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"ShowDetails_{showId}";

            if (_cache.TryGetValue(cacheKey, out TVShowDto cachedShowDetails))
            {
                _logger.LogInformation("Returning show details from cache");
                return cachedShowDetails;
            }

            var queryParams = new Dictionary<string, string>();
            var show = await GetAsync<TMDbShowDetailsResponse>($"tv/{showId}", queryParams, cancellationToken);

            var showDetails = _mapper.Map<TVShowDto>(show);
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, showDetails, cacheEntryOptions);

            return showDetails;
        }

        public async Task<SeasonDto> GetShowSeasonDetailsAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"ShowSeasonDetails_{showId}_{seasonNumber}";
            if (_cache.TryGetValue(cacheKey, out SeasonDto cachedEpisodes))
            {
                _logger.LogInformation($"Returning show details from cache for show {showId}, season {seasonNumber}");
                return cachedEpisodes;
            }

            var queryParams = new Dictionary<string, string> { ["api_key"] = _options.ApiKey };
            var seasonResponse = await GetAsync<TMDbSeasonResponse>($"tv/{showId}/season/{seasonNumber}", queryParams, cancellationToken);

            var season = _mapper.Map<SeasonDto>(seasonResponse);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, season, cacheEntryOptions);

            return season;
        }

        public async Task<EpisodeDto> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"EpisodeDetails_{showId}_{seasonNumber}_{episodeNumber}";

            if (_cache.TryGetValue(cacheKey, out EpisodeDto cachedEpisode))
            {
                _logger.LogInformation("Returning episode from cache");
                return cachedEpisode;
            }

            var queryParams = new Dictionary<string, string> { ["api_key"] = _options.ApiKey };
            var episodeResponse = await GetAsync<TMDbEpisodeResponse>($"tv/{showId}/season/{seasonNumber}/episode/{episodeNumber}", queryParams, cancellationToken);
            var mappedEpisode = _mapper.Map<EpisodeDto>(episodeResponse);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, mappedEpisode, cacheEntryOptions);

            return mappedEpisode;
        }

        public async Task<IEnumerable<TVShowDto>> SearchShowsAsync(string query, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["api_key"] = _options.ApiKey,
                ["query"] = query
            };

            var searchResponse = await GetAsync<TMDbSearchResponse>("search/tv", queryParams, cancellationToken);
            return _mapper.Map<IEnumerable<TVShowDto>>(searchResponse.Results);
        }

        private async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("TMDbApi");
            var uriBuilder = new UriBuilder($"{_options.BaseUrl}{endpoint}");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();

            try
            {
                _logger.LogInformation($"Sending GET request to: {uriBuilder.Uri}");
                var response = await client.GetAsync(uriBuilder.Uri, cancellationToken);

                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug($"Received response: {jsonContent}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var result = JsonSerializer.Deserialize<T>(jsonContent, options);
                if (result == null)
                {
                    throw new JsonException("Deserialization resulted in null object");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error occurred while calling TMDb API. Endpoint: {endpoint}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error deserializing TMDb API response. Endpoint: {endpoint}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while calling TMDb API. Endpoint: {endpoint}");
                throw;
            }
        }

    }
}