using TVShowTracker.Domain.Common;

namespace TVShowTracker.Domain.Entities;

public class User : AggregateRoot
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    private List<Watchlist> _watchlist = new List<Watchlist>();
    public IReadOnlyCollection<Watchlist> Watchlist => _watchlist.AsReadOnly();

    public User(string username, string password, string email)
    {
        Username = username;
        Password = password; // Note: In real-world scenarios, ensure this is hashed
        Email = email;
    }

    public void AddToWatchlist(TopShow show)
    {
        if (!_watchlist.Any(w => w.Show.Id == show.Id))
        {
            _watchlist.Add(new Watchlist(this, show));
        }
    }
}
