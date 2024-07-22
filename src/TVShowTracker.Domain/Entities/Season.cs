namespace TVShowTracker.Domain.Entities;

public class Season : BaseEntity
{
    public int TMDbSeasonId { get; set; }
    public int ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AirDate { get; set; }
    public int EpisodeCount { get; set; } = 0;
    public int SeasonNumber { get; set; }
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;

    public TVShow? TVShow { get; set; }
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}


