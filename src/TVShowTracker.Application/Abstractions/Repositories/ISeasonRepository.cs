namespace TVShowTracker.Application.Abstractions.Repositories
{
    public interface ISeasonRepository
    {
        Task<Season?> GetSeasonAsync(int showId, int seasonNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Season>> GetSeasonsForShowAsync(int showId, CancellationToken cancellationToken = default);
        Task AddOrUpdateAsync(Season season, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<Season> seasons, CancellationToken cancellationToken = default);
    }
}
