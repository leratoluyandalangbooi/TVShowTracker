namespace TVShowTracker.Domain.Entities;

public class Episode : BaseEntity
{
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AirDate { get; set; }
    public int Runtime { get; set; }
    public string Overview { get; set; } = string.Empty;
    public string StillPath { get; set; } = string.Empty;

    public Show? Show { get; set; }
}
