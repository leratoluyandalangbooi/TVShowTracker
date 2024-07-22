using AutoMapper;
using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Tests.Services
{
    public class WatchedEpisodeServiceTests
    {
        private readonly Mock<IWatchedEpisodeRepository> _mockWatchedEpisodeRepo;
        private readonly Mock<IEpisodeRepository> _mockEpisodeRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<WatchedEpisodeService>> _mockLogger;
        private readonly WatchedEpisodeService _service;

        public WatchedEpisodeServiceTests()
        {
            _mockWatchedEpisodeRepo = new Mock<IWatchedEpisodeRepository>();
            _mockEpisodeRepo = new Mock<IEpisodeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<WatchedEpisodeService>>();

            _service = new WatchedEpisodeService(
                _mockWatchedEpisodeRepo.Object,
                _mockEpisodeRepo.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetUserWatchedEpisodesAsync_ShouldReturnMappedWatchedEpisodes()
        {
            int userId = 1;
            var watchedEpisodes = new List<WatchedEpisode> { new WatchedEpisode() };
            var watchedEpisodeDtos = new List<WatchedEpisodeDto> { new WatchedEpisodeDto() };

            _mockWatchedEpisodeRepo.Setup(repo => repo.GetUserWatchedEpisodesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(watchedEpisodes);
            _mockMapper.Setup(m => m.Map<IEnumerable<WatchedEpisodeDto>>(watchedEpisodes))
                .Returns(watchedEpisodeDtos);

            var result = await _service.GetUserWatchedEpisodesAsync(userId);

            Assert.Equal(watchedEpisodeDtos, result);
        }

        [Fact]
        public async Task MarkEpisodeAsWatchedAsync_ShouldAddWatchedEpisode_WhenNotAlreadyWatched()
        {
            int userId = 1, episodeId = 1;

            _mockWatchedEpisodeRepo.Setup(repo => repo.IsEpisodeWatchedAsync(userId, episodeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _service.MarkEpisodeAsWatchedAsync(userId, episodeId);

            _mockWatchedEpisodeRepo.Verify(repo => repo.AddWatchedEpisodeAsync(It.IsAny<WatchedEpisode>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkEpisodeAsWatchedAsync_ShouldThrowException_WhenAlreadyWatched()
        {
            int userId = 1, episodeId = 1;

            _mockWatchedEpisodeRepo.Setup(repo => repo.IsEpisodeWatchedAsync(userId, episodeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkEpisodeAsWatchedAsync(userId, episodeId));
        }

        [Fact]
        public async Task UnmarkEpisodeAsWatchedAsync_ShouldRemoveWatchedEpisode()
        {
            int userId = 1, episodeId = 1;

            await _service.UnmarkEpisodeAsWatchedAsync(userId, episodeId);

            _mockWatchedEpisodeRepo.Verify(repo => repo.RemoveWatchedEpisodeAsync(userId, episodeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWatchedEpisodeAsync_ShouldUpdateWatchedEpisode_WhenExists()
        {
            int userId = 1, episodeId = 1;
            var watchedEpisode = new WatchedEpisode();

            _mockWatchedEpisodeRepo.Setup(repo => repo.GetWatchedEpisodeAsync(userId, episodeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(watchedEpisode);

            await _service.UpdateWatchedEpisodeAsync(userId, episodeId);

            _mockWatchedEpisodeRepo.Verify(repo => repo.UpdateWatchedEpisodeAsync(It.IsAny<WatchedEpisode>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWatchedEpisodeAsync_ShouldThrowException_WhenNotExists()
        {
            int userId = 1, episodeId = 1;

            _mockWatchedEpisodeRepo.Setup(repo => repo.GetWatchedEpisodeAsync(userId, episodeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((WatchedEpisode)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateWatchedEpisodeAsync(userId, episodeId));
        }

        [Fact]
        public async Task IsEpisodeWatchedAsync_ShouldReturnCorrectValue()
        {
            int userId = 1, episodeId = 1;
            bool isWatched = true;

            _mockWatchedEpisodeRepo.Setup(repo => repo.IsEpisodeWatchedAsync(userId, episodeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(isWatched);

            var result = await _service.IsEpisodeWatchedAsync(userId, episodeId);

            Assert.Equal(isWatched, result);
        }
    }
}