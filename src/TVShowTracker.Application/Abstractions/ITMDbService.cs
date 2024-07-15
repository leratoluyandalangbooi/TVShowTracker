namespace TVShowTracker.Application.Interfaces;

public interface ITMDbService
{
    Task<IEnumerable<Show>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20);
    Task<Show?> GetShowDetailsAsync(int showId);
    Task<Episode?> GetEpisodeDetailsAsync(int showId, int seasonNumber, int episodeNumber);
    Task<IEnumerable<Show>> SearchShowsAsync(string query);
}
