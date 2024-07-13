using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TVShowTracker.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    private readonly IShowRepository _showRepository;

    public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IOptions<JwtSettings> jwtSettings, IShowRepository showRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
        _showRepository = showRepository;
    }

    public async Task<bool> RegisterUserAsync(string username, string password, string email)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            return false;
        }

        var user = new User(username, _passwordHasher.HashPassword(null!, password), email);
        await _userRepository.AddAsync(user);
        return true;
    }

    public async Task<string> LoginUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return null!;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null!;
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_jwtSettings.ExpirationInDays);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task AddToWatchlistAsync(int userId, int showId)
    {
        //var user = await _userRepository.GetByIdAsync(userId);
        //var show = await _showRepository.GetByIdAsync(showId);

        //if (user == null || show == null)
        //{
        //    //throw new NotFoundException("User or Show not found.");
        //}

        //user.AddToWatchlist(show);
        //await _userRepository.UpdateAsync(user);
    }
}
