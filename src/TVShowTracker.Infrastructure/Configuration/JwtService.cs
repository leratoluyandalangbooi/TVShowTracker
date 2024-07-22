using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TVShowTracker.Infrastructure.Configuration;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private const string AuthScheme = "JWT";

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string GenerateToken(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        string key = _options.Key ?? throw new InvalidOperationException("JWT Key is not configured.");
        string issuer = _options.Issuer ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        string audience = _options.Audience ?? throw new InvalidOperationException("JWT Audience is not configured.");
        int expirationHours = _options.ExpirationHours;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTimeOffset.UtcNow.AddHours(expirationHours).DateTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}