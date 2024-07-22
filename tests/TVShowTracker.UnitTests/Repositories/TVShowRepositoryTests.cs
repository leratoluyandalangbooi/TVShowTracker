using Microsoft.Extensions.Logging.Abstractions;
using TVShowTracker.Infrastructure.Repositories;

namespace TVShowTracker.UnitTests.Repositories;

public class TVShowRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TVShowRepository> _logger;
    private readonly TVShowRepository _repository;

    public TVShowRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _logger = new NullLogger<TVShowRepository>();
        _repository = new TVShowRepository(_context, _logger);
    }

    [Fact]
    public async Task GetTopShowsAsync_ReturnsTopShows()
    {
        // Arrange
        await _context.TVShows.AddRangeAsync(new List<TVShow>
        {
            new TVShow { TMDbShowId = 1, Name = "Show 1", Popularity = 9.5 },
            new TVShow { TMDbShowId = 2, Name = "Show 2", Popularity = 8.5 },
            new TVShow { TMDbShowId = 3, Name = "Show 3", Popularity = 7.5 }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTopShowsAsync(2);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(9.5, result.First().Popularity);
    }

    [Fact]
    public async Task GetShowByIdAsync_ReturnsShow()
    {
        // Arrange
        var show = new TVShow { TMDbShowId = 123, Name = "Test Show" };
        await _context.TVShows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetShowByIdAsync(123);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.TMDbShowId);
    }

    [Fact]
    public async Task AddOrUpdateAsync_AddsNewShow()
    {
        // Arrange
        var show = new TVShow { TMDbShowId = 123, Name = "New Show" };

        // Act
        await _repository.AddOrUpdateAsync(show);

        // Assert
        var addedShow = await _context.TVShows.FirstOrDefaultAsync(s => s.TMDbShowId == 123);
        Assert.NotNull(addedShow);
        Assert.Equal("New Show", addedShow.Name);
    }

    [Fact]
    public async Task AddRangeAsync_AddsMultipleShows()
    {
        // Arrange
        var shows = new List<TVShow>
        {
            new TVShow { TMDbShowId = 1, Name = "Show 1" },
            new TVShow { TMDbShowId = 2, Name = "Show 2" }
        };

        // Act
        await _repository.AddRangeAsync(shows);

        // Assert
        var result = await _context.TVShows.ToListAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchShowsAsync_ReturnsMatchingShows()
    {
        // Arrange
        await _context.TVShows.AddRangeAsync(new List<TVShow>
        {
            new TVShow { TMDbShowId = 1, Name = "Breaking Bad" },
            new TVShow { TMDbShowId = 2, Name = "Better Call Saul" },
            new TVShow { TMDbShowId = 3, Name = "The Walking Dead" }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchShowsAsync("Bad");

        // Assert
        Assert.Single(result);
        Assert.Equal("Breaking Bad", result.First().Name);
    }

    [Fact]
    public async Task SearchShowsAsync_ThrowsExceptionForEmptyQuery()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.SearchShowsAsync(""));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}