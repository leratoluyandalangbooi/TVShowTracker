namespace TVShowTracker.Domain.Entities;

public class Watchlist : BaseEntity
{
    public int UserId { get; set; }
    public int ShowId { get; set; }
    public int? EpisodeId { get; set; }
    public DateTime AddedDate { get; set; }

    public User? User { get; set; }
    public Show? Show { get; set; }
    public Episode? Episode { get; set; }
}
