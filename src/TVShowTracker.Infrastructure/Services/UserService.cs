using CryptoHelper;
using TVShowTracker.Application.DTOs.Entities;
using TVShowTracker.Application.DTOs.Responses;

namespace TVShowTracker.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IJwtService jwtService, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _mapper = mapper;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserDto> RegisterUserAsync(string username, string email, string password, string preferredName, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username, email, and password are required.");
        }

        if (await _userRepository.UsernameExistsAsync(username))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        if (await _userRepository.EmailExistsAsync(email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        try
        {
            var user = new UserDto
            {
                Username = username,
                Email = email,
                PasswordHash = Crypto.HashPassword(password),
                PreferredName = string.IsNullOrWhiteSpace(preferredName) ? username : preferredName,
                CreatedAt = DateTime.UtcNow,
                IsAdmin = isAdmin
            };

            var userEntity = _mapper.Map<User>(user);
            await _userRepository.CreateAsync(userEntity, cancellationToken);

            _logger.LogInformation($"User registered successfully: {username}.");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during user registration: {username}.");
            throw;
        }
    }

    public async Task<LoginResponseDto> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null || !Crypto.VerifyHashedPassword(user.PasswordHash, password))
            {
                _logger.LogWarning($"Invalid email or password");
                throw new Exception("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin,
                    PreferredName = user.PreferredName
                }
            };

            _logger.LogInformation($"User authenticated successfully: {email}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during authentication for username: {email}");
            return null!;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task UpdateUserAsync(int id, string email, string preferredName, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
        {
            if (await _userRepository.EmailExistsAsync(email, cancellationToken))
            {
                throw new InvalidOperationException("Email already exists.");
            }
            user.Email = email;
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.PreferredName = preferredName;

        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation($"User updated successfully: {user.Username}");
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        try
        {
            if (!Crypto.VerifyHashedPassword(user.PasswordHash, currentPassword))
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            user.PasswordHash = Crypto.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation($"Password changed successfully for user: {user.Username}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during password change for user ID: {userId}");
            throw;
        }
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        await _userRepository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation($"User deleted successfully: {id}");
    }
}
