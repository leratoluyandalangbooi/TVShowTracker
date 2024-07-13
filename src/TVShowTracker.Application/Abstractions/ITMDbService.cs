namespace TVShowTracker.Application.Interfaces;

public interface ITMDbService
{
    Task<List<TopShowDto>?> GetTopShowsAsync(string language = "en-US", int page = 1, int pageSize = 20);
}
