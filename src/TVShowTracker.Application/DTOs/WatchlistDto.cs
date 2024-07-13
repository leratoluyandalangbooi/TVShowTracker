namespace TVShowTracker.Application.DTOs;

public class WatchlistDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowId { get; set; }
    public bool Watched { get; set; }
    public int? LastWatchedEpisodeId { get; set; }

    public WatchlistDto(Watchlist watchlist)
    {
        Id = watchlist.Id;
        UserId = watchlist.UserId;
        ShowId = watchlist.ShowId;
        Watched = watchlist.Watched;
        LastWatchedEpisodeId = watchlist.LastWatchedEpisodeId;
    }
}
