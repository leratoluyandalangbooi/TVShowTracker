using System.Text.Json.Serialization;

namespace TVShowTracker.Domain.Entities;

public class TopShow : AggregateRoot
{
    //public string Title { get; private set; }
    //public string Genre { get; private set; }
    //public string Description { get; private set; }
    //public DateTime? ReleaseDate { get; private set; }
    //public string ImageUrl { get; private set; }
    //private List<Episode> _episodes = new List<Episode>();
    //public IReadOnlyCollection<Episode> Episodes => _episodes.AsReadOnly();
    //public List<Watchlist> _watchlists = new List<Watchlist>();

    //public Show(string title, string genre, string? description = null, DateTime? releaseDate = null, string? imageUrl = null)
    //{
    //    Title = title;
    //    Genre = genre;
    //    Description = description ?? string.Empty;
    //    ReleaseDate = releaseDate;
    //    ImageUrl = imageUrl ?? string.Empty;
    //}

    //public void AddEpisode(Episode episode)
    //{
    //    if (!_episodes.Any(e => e.SeasonNumber == episode.SeasonNumber && e.EpisodeNumber == episode.EpisodeNumber))
    //    {
    //        _episodes.Add(episode);
    //    }
    //}

    //public double GetWatchedPercentage()
    //{
    //    if (_watchlists.Count == 0) return 0;
    //    return (double)_watchlists.Count(e => e.Watched) / _watchlists.Count * 100;
    //}


    public string Name { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public DateTime FirstAirDate { get; set; }

    public double VoteAverage { get; set; }

    public int VoteCount { get; set; }

    public bool Adult { get; set; }

    public string BackdropPath { get; set; } = string.Empty;

    public List<int> GenreIds { get; set; } = new List<int>();

    public List<string> OriginCountry { get; set; } = new List<string>();

    public string OriginalLanguage { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public double Popularity { get; set; }

    public string PosterPath { get; set; } = string.Empty;
}
