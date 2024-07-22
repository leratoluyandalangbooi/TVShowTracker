using TVShowTracker.API.Extensions;
using TVShowTracker.Application.Abstractions.Services;

namespace TVShowTracker.API.Endpoints;

public static class WatchedEpisodeEndpoints
{
    public static void MapWatchedEpisodeEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/watched-episodes")
            .WithTags("Watched Episodes")
            .RequireAuthorization()
            .MapWatchedEpisodeEndpoints();
    }

    private static RouteGroupBuilder MapWatchedEpisodeEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetWatchedEpisodes);
        group.MapPost("/{episodeId}", MarkEpisodeAsWatched);
        group.MapDelete("/{episodeId}", UnmarkEpisodeAsWatched);
        group.MapGet("/{episodeId}/is-watched", IsEpisodeWatched);
        group.MapPut("/{episodeId}", UpdateWatchedEpisode);
        return group;
    }

    private static async Task<IResult> GetWatchedEpisodes(HttpContext context, IWatchedEpisodeService watchedEpisodeService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        var watchedEpisodes = await watchedEpisodeService.GetUserWatchedEpisodesAsync(userId, cancellationToken);
        return Results.Ok(watchedEpisodes);
    }

    private static async Task<IResult> MarkEpisodeAsWatched(int episodeId, HttpContext context, IWatchedEpisodeService watchedEpisodeService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        try
        {
            await watchedEpisodeService.MarkEpisodeAsWatchedAsync(userId, episodeId, cancellationToken);
            return Results.Ok(new { message = "Episode marked as watched successfully" });
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

    private static async Task<IResult> UnmarkEpisodeAsWatched(int episodeId, HttpContext context, IWatchedEpisodeService watchedEpisodeService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        await watchedEpisodeService.UnmarkEpisodeAsWatchedAsync(userId, episodeId, cancellationToken);
        return Results.Ok(new { message = "Episode unmarked as watched successfully" });
    }

    private static async Task<IResult> IsEpisodeWatched(int episodeId, HttpContext context, IWatchedEpisodeService watchedEpisodeService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        var isWatched = await watchedEpisodeService.IsEpisodeWatchedAsync(userId, episodeId, cancellationToken);
        return Results.Ok(new { isWatched });
    }

    private static async Task<IResult> UpdateWatchedEpisode(int episodeId, HttpContext context, IWatchedEpisodeService watchedEpisodeService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        try
        {
            await watchedEpisodeService.UpdateWatchedEpisodeAsync(userId, episodeId, cancellationToken);
            return Results.Ok(new { message = "Watched episode updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
}