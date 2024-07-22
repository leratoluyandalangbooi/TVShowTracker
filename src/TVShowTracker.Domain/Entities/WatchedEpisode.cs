namespace TVShowTracker.Domain.Entities;

public class WatchedEpisode : BaseEntity
{
    public int UserId { get; set; }
    public int EpisodeId { get; set; }
    public DateTime WatchedDate { get; set; }

    public User? User { get; set; }
    public Episode? Episode { get; set; }
}
