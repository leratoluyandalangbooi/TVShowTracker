namespace TVShowTracker.Application.Abstractions;

public interface IShowRepository
{
    Task<IEnumerable<Show>> GetTopShowsAsync(int pageSize = 20);
    Task<Show?> GetShowByIdAsync(int id);
    Task AddOrUpdateAsync(Show show);
    Task AddRangeAsync(IEnumerable<Show> shows);
}
