namespace TVShowTracker.Domain.Entities;

public class Show : BaseEntity
{
    public int ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime FirstAirDate { get; set; }
    public double Popularity { get; set; }
    public string PosterPath { get; set; } = string.Empty;

    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    public ICollection<Watchlist> WatchlistItems { get; set; } = new HashSet<Watchlist>();

}