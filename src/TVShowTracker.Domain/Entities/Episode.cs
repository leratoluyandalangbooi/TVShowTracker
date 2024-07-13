namespace TVShowTracker.Domain.Entities;

public class Episode : Entity
{
    public int ShowId { get; private set; }
    public int SeasonNumber { get; private set; }
    public int EpisodeNumber { get; private set; }
    public string Title { get; private set; }
    public DateTime? AirDate { get; private set; }

    public Episode(int showId, int seasonNumber, int episodeNumber, string title, DateTime? airDate = null)
    {
        ShowId = showId;
        SeasonNumber = seasonNumber;
        EpisodeNumber = episodeNumber;
        Title = title;
        AirDate = airDate;
    }
}
