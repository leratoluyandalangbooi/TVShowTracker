namespace TVShowTracker.Application.DTOs.Entities;

public class UserDto : BaseEntityDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    public ICollection<WatchedEpisodeDto> WatchedEpisodes { get; set; } = new List<WatchedEpisodeDto>();
}
