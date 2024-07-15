using TVShowTracker.Application.Interfaces;

namespace TVShowTracker.API.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/search", async (string query, ITMDbService tmdbService) =>
        {
            var results = await tmdbService.SearchShowsAsync(query);
            return Results.Ok(results);
        })
        .WithName("SearchShows")
        .WithTags("Search");
    }
}
