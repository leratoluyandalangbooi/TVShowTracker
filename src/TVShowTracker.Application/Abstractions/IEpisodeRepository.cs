namespace TVShowTracker.Application.Abstractions;

public interface IEpisodeRepository
{
    Task<Episode?> GetEpisodeAsync(int showId, int seasonNumber, int episodeNumber);
    Task AddOrUpdateAsync(Episode episode);
}
