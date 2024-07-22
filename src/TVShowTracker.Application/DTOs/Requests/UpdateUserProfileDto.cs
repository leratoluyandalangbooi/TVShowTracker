namespace TVShowTracker.Application.DTOs.Request;

public class UpdateUserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;
}