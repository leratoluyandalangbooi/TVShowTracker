namespace TVShowTracker.Infrastructure.Settings;

public class TMDbOptions
{
    public string BaseUrl { get; set; } = "https://api.themoviedb.org/3/";
    public string ApiKey { get; set; } = string.Empty;
    public string ReadAccessToken { get; set; } = string.Empty;
    public int RequestsPerSecond { get; set; } = 10; // TMDb allows around +40 per sec
}
