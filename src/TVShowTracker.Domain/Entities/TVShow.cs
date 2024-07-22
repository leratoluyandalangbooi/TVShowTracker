namespace TVShowTracker.Domain.Entities;

public class TVShow : BaseEntity
{
    public int TMDbShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime FirstAirDate { get; set; }
    public double Popularity { get; set; }
    public string PosterPath { get; set; } = string.Empty;

    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}
