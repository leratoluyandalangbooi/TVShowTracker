﻿namespace TVShowTracker.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PreferredName { get; set; } = string.Empty;

    public ICollection<Watchlist> Watchlist { get; set; } = new List<Watchlist>();
}
