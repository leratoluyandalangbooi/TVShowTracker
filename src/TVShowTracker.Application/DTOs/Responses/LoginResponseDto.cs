using TVShowTracker.Application.DTOs.Entities;

namespace TVShowTracker.Application.DTOs.Responses;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}
