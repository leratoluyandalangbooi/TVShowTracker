namespace TVShowTracker.Domain.Entities;

public class Watchlist : Entity
{
    public int UserId { get; private set; }
    public User User { get; private set; }
    public int ShowId { get; private set; }
    public TopShow Show { get; private set; }
    public bool Watched { get; private set; }
    public int? LastWatchedEpisodeId { get; private set; }
    public Episode LastWatchedEpisode { get; private set; }

    public Watchlist(User user, TopShow show)
    {
        User = user;
        UserId = user.Id;
        Show = show;
        ShowId = show.Id;
        Watched = false;
    }

    public void MarkAsWatched()
    {
        Watched = true;
    }

    public void UpdateLastWatchedEpisode(Episode episode)
    {
        LastWatchedEpisode = episode;
        LastWatchedEpisodeId = episode.Id;
    }
}
