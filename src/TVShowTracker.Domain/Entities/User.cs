namespace TVShowTracker.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    public ICollection<WatchedEpisode> WatchedEpisodes { get; set; } = new List<WatchedEpisode>();
}
