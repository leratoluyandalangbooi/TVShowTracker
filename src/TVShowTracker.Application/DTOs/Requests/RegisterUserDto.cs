namespace TVShowTracker.Application.DTOs.Request;

public class RegisterUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;
}
