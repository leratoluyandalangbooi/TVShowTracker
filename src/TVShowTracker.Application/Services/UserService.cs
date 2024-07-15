using System.Security.Cryptography;

namespace TVShowTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> RegisterUserAsync(string username, string email, string password, string preferredName)
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

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            PreferredName = string.IsNullOrWhiteSpace(preferredName) ? username : preferredName,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        _logger.LogInformation($"User registered successfully: {username}");

        return user;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning($"Failed login attempt for username: {username}");
            return null!;
        }

        _logger.LogInformation($"User authenticated successfully: {username}");
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task UpdateUserAsync(int id, string email, string preferredName)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
        {
            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("Email already exists.");
            }
            user.Email = email;
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.PreferredName = preferredName;

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation($"User updated successfully: {user.Username}");
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!VerifyPassword(currentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.PasswordHash = HashPassword(newPassword);

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation($"Password changed successfully for user: {user.Username}");
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
        _logger.LogInformation($"User deleted successfully: {id}");
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }
    }
}
