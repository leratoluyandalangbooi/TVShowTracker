namespace TVShowTracker.Application.Interfaces;

public interface IShowService
{
    Task SaveTopShowsToDatabaseAsync(List<TopShowDto> topShows);
    Task<List<TopShowDto>> GetTopShowsFromDatabase(int page, int pageSize);
}
