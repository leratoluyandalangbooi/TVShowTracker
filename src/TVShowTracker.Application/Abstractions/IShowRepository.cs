namespace TVShowTracker.Application.Interfaces;

public interface IShowRepository
{
    Task<List<TopShow>> GetTopShowsFromDatabase();
    Task<TopShow?> GetTopShowByIdAsync(int id);
    Task AddAsync(TopShow show);
    Task SaveAsync();
}
