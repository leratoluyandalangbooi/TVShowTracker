namespace TVShowTracker.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(User user);
}
