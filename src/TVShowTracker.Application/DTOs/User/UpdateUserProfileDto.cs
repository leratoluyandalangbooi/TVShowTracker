namespace TVShowTracker.Application.DTOs.User;

public class UpdateUserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;
}