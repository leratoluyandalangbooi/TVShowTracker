namespace TVShowTracker.Application.Abstractions;

public interface ITMDbApiClient
{
    Task<List<Show>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20);
    Task<Show?> GetShowDetailsAsync(int showId);
    Task<Episode?> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber);
    Task<List<Show>?> SearchShowsAsync(string query);
}
