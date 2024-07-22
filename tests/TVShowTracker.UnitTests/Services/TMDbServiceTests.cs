using AutoMapper;
using System.Text.Json;
using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Tests.Services;

public class TMDbServiceTests
{
    private readonly Mock<ITMDbApiClient> _mockTmdbApiClient;
    private readonly Mock<ITVShowRepository> _mockShowsRepository;
    private readonly Mock<ISeasonRepository> _mockSeasonRepository;
    private readonly Mock<IEpisodeRepository> _mockEpisodeRepository;
    private readonly Mock<ILogger<TMDbService>> _mockLogger;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TMDbService _service;

    public TMDbServiceTests()
    {
        _mockTmdbApiClient = new Mock<ITMDbApiClient>();
        _mockShowsRepository = new Mock<ITVShowRepository>();
        _mockSeasonRepository = new Mock<ISeasonRepository>();
        _mockEpisodeRepository = new Mock<IEpisodeRepository>();
        _mockLogger = new Mock<ILogger<TMDbService>>();
        _mockCache = new Mock<IDistributedCache>();
        _mockMapper = new Mock<IMapper>();

        _service = new TMDbService(
            _mockTmdbApiClient.Object,
            _mockShowsRepository.Object,
            _mockSeasonRepository.Object,
            _mockEpisodeRepository.Object,
            _mockLogger.Object,
            _mockCache.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetTopShowsAsync_CacheHit_ReturnsFromCache()
    {
        string language = "en";
        int page = 1;
        int pageSize = 20;
        var cachedShows = new List<TVShowDto> { new TVShowDto { TMDbShowId = 1, Name = "Cached Show" } };
        var cachedShowsJson = JsonSerializer.Serialize(cachedShows);
        var cachedShowsBytes = System.Text.Encoding.UTF8.GetBytes(cachedShowsJson);

        _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(cachedShowsBytes);

        var result = await _service.GetTopShowsAsync(language, page, pageSize);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Cached Show", result.First().Name);

        // Verify that the API client was not called
        _mockTmdbApiClient.Verify(c => c.GetTopShowsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task GetTopShowsAsync_CacheMiss_FetchesFromApiAndCaches()
    {
        string language = "en";
        int page = 1;
        int pageSize = 20;
        var apiShows = new List<TVShowDto> { new TVShowDto { TMDbShowId = 1, Name = "API Show" } };

        _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[])null);

        _mockTmdbApiClient.Setup(c => c.GetTopShowsAsync(language, page, pageSize, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(apiShows);

        var result = await _service.GetTopShowsAsync(language, page, pageSize);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("API Show", result.First().Name);

        // Verify that the API client was called
        _mockTmdbApiClient.Verify(c => c.GetTopShowsAsync(language, page, pageSize, It.IsAny<CancellationToken>()), Times.Once);

        // Verify that the result was cached
        _mockCache.Verify(c => c.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetShowDetailsAsync_CacheMiss_FetchesFromApiAndCaches()
    {
        int showId = 1;
        var apiShow = new TVShowDto { TMDbShowId = showId, Name = "API Show" };

        _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[])null!);

        _mockTmdbApiClient.Setup(c => c.GetShowDetailsAsync(showId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(apiShow);

        var showEntity = new TVShow { TMDbShowId = showId, Name = "API Show" };
        _mockMapper.Setup(m => m.Map<TVShow>(It.IsAny<TVShowDto>())).Returns(showEntity);

        var result = await _service.GetShowDetailsAsync(showId);

        Assert.NotNull(result);
        Assert.Equal(showId, result.TMDbShowId);
        Assert.Equal("API Show", result.Name);

        // Verify that the API client was called
        _mockTmdbApiClient.Verify(c => c.GetShowDetailsAsync(showId, It.IsAny<CancellationToken>()), Times.Once);

        // Verify that the result was cached
        _mockCache.Verify(c => c.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify that the show was added or updated in the repository
        _mockShowsRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<TVShow>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchShowsAsync_ApiReturnsResults_ReturnsApiResults()
    {
        var apiResults = new List<TVShowDto> { new TVShowDto { Id = 1, Name = "Test Show" } };
        _mockTmdbApiClient.Setup(c => c.SearchShowsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResults);

        var result = await _service.SearchShowsAsync("test");

        Assert.Equal(apiResults.Count, result.Count());
        Assert.Equal(apiResults[0].Id, result.First().Id);
        Assert.Equal(apiResults[0].Name, result.First().Name);
        _mockShowsRepository.Verify(r => r.SearchShowsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchShowsAsync_ApiReturnsNoResults_SearchesDatabase()
    {
        var dbResults = new List<TVShow> { new TVShow { Id = 1, Name = "Test Show" } };
        _mockTmdbApiClient.Setup(c => c.SearchShowsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TVShowDto>());
        _mockShowsRepository.Setup(r => r.SearchShowsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbResults);
        _mockMapper.Setup(m => m.Map<IEnumerable<TVShowDto>>(It.IsAny<IEnumerable<TVShow>>()))
            .Returns(dbResults.Select(s => new TVShowDto { Id = s.Id, Name = s.Name }));

        var result = await _service.SearchShowsAsync("test");

        Assert.Equal(dbResults.Count, result.Count());
        Assert.Equal(dbResults[0].Id, result.First().Id);
        Assert.Equal(dbResults[0].Name, result.First().Name);
    }

    [Fact]
    public async Task InvalidateCacheAsync_ValidKey_RemovesFromCache()
    {
        string cacheKey = "test_key";

        await _service.InvalidateCacheAsync(cacheKey);

        _mockCache.Verify(c => c.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
    }
}