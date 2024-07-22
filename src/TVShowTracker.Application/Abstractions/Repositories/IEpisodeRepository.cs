namespace TVShowTracker.Application.Abstractions.Repositories;

public interface IEpisodeRepository
{
    Task<Episode?> GetEpisodeAsync(int showId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Episode>> GetEpisodesForSeasonAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(Episode episode, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Episode> episodes, CancellationToken cancellationToken = default);
}