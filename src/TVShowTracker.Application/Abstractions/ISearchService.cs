namespace TVShowTracker.Application.Abstractions;

public interface ISearchService
{
    Task<IEnumerable<Show>> SearchShowsAsync(string query);
    Task IndexShowAsync(Show show);
    Task IndexShowsAsync(IEnumerable<Show> shows);
}
