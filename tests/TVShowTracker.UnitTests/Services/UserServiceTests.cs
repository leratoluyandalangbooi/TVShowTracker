using AutoMapper;
using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _service = new UserService(
                _mockUserRepo.Object,
                _mockJwtService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUser_WhenValidInputProvided()
        {
            string username = "testuser";
            string email = "test@example.com";
            string password = "password123";
            string preferredName = "Test User";

            _mockUserRepo.Setup(repo => repo.UsernameExistsAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockUserRepo.Setup(repo => repo.EmailExistsAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserDto>())).Returns(new User());

            var result = await _service.RegisterUserAsync(username, email, password, preferredName);

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(email, result.Email);
            Assert.Equal(preferredName, result.PreferredName);
            _mockUserRepo.Verify(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameExists()
        {
            string username = "existinguser";
            _mockUserRepo.Setup(repo => repo.UsernameExistsAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RegisterUserAsync(username, "test@example.com", "password", "Test User"));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            string username = "validuser";
            string password = "password123";
            var user = new User { Id = 1, Username = username, PasswordHash = CryptoHelper.Crypto.HashPassword(password) };
            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockJwtService.Setup(jwt => jwt.GenerateToken(It.IsAny<User>())).Returns("token");
            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = 1, Username = username });

            var result = await _service.AuthenticateAsync(username, password);

            Assert.NotNull(result);
            Assert.Equal("token", result.Token);
            Assert.Equal(username, result?.User?.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            int userId = 1;
            var user = new User { Id = userId, Username = "testuser" };
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Id = userId, Username = "testuser" });

            var result = await _service.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            int userId = 1;
            string newEmail = "newemail@example.com";
            string newPreferredName = "New Name";
            var user = new User { Id = userId, Username = "testuser", Email = "old@example.com" };
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockUserRepo.Setup(repo => repo.EmailExistsAsync(newEmail, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await _service.UpdateUserAsync(userId, newEmail, newPreferredName);

            Assert.Equal(newEmail, user.Email);
            Assert.Equal(newPreferredName, user.PreferredName);
            _mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldChangePassword_WhenCurrentPasswordIsCorrect()
        {
            int userId = 1;
            string currentPassword = "oldpassword";
            string newPassword = "newpassword";
            var user = new User { Id = userId, Username = "testuser", PasswordHash = CryptoHelper.Crypto.HashPassword(currentPassword) };
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            await _service.ChangePasswordAsync(userId, currentPassword, newPassword);

            Assert.True(CryptoHelper.Crypto.VerifyHashedPassword(user.PasswordHash, newPassword));
            _mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            int userId = 1;

            await _service.DeleteUserAsync(userId);

            _mockUserRepo.Verify(repo => repo.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}