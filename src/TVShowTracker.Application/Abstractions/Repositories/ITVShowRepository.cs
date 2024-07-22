namespace TVShowTracker.Application.Abstractions.Repositories;

public interface ITVShowRepository
{
    Task<IEnumerable<TVShow>> GetTopShowsAsync(int? pageSize = null, CancellationToken cancellationToken = default);
    Task<TVShow?> GetShowByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(TVShow show, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TVShow> shows, CancellationToken cancellationToken = default);
    Task<IEnumerable<TVShow>> SearchShowsAsync(string query, CancellationToken cancellationToken = default);
}
