using TVShowTracker.Application.Abstractions;
using TVShowTracker.API.Extensions;

namespace TVShowTracker.API.Endpoints;

public static class WatchlistEndpoints
{
    public static void MapWatchlistEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/watchlist")
            .WithTags("Watchlist")
            .RequireAuthorization()
            .MapWatchlistEndpoints();
    }

    private static RouteGroupBuilder MapWatchlistEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetWatchlist);
        group.MapPost("/{showId}", AddToWatchlist);
        group.MapDelete("/{showId}", RemoveFromWatchlist);
        group.MapPut("/{showId}/watched-episode/{episodeId}", UpdateWatchedEpisode);
        group.MapGet("/{showId}/in-watchlist", IsShowInWatchlist);

        return group;
    }

    private static async Task<IResult> GetWatchlist(HttpContext context, IWatchlistService watchlistService)
    {
        var userId = context.GetUserId();
        var watchlist = await watchlistService.GetUserWatchlistAsync(userId);
        return Results.Ok(watchlist);
    }

    private static async Task<IResult> AddToWatchlist(int showId, HttpContext context, IWatchlistService watchlistService)
    {
        var userId = context.GetUserId();
        try
        {
            await watchlistService.AddToWatchlistAsync(userId, showId);
            return Results.Ok(new { message = "Show added to watchlist successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> RemoveFromWatchlist(int showId, HttpContext context, IWatchlistService watchlistService)
    {
        var userId = context.GetUserId();
        await watchlistService.RemoveFromWatchlistAsync(userId, showId);
        return Results.Ok(new { message = "Show removed from watchlist successfully" });
    }

    private static async Task<IResult> UpdateWatchedEpisode(int showId, int episodeId, HttpContext context, IWatchlistService watchlistService)
    {
        var userId = context.GetUserId();
        try
        {
            await watchlistService.UpdateWatchedEpisodeAsync(userId, showId, episodeId);
            return Results.Ok(new { message = "Watched episode updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> IsShowInWatchlist(int showId, HttpContext context, IWatchlistService watchlistService)
    {
        var userId = context.GetUserId();
        var isInWatchlist = await watchlistService.IsShowInWatchlistAsync(userId, showId);
        return Results.Ok(new { isInWatchlist });
    }
}
