﻿namespace TVShowTracker.Application.Settings;

public class JwtOptions
{
    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpirationHours { get; set; } = 5;
}
