using Microsoft.AspNetCore.Mvc;
using TVShowTracker.API.Extensions;
using TVShowTracker.Application.Abstractions.Repositories;
using TVShowTracker.Application.Abstractions.Services;
using TVShowTracker.Domain.Entities;

namespace TVShowTracker.API.Endpoints;

public static class ShowEndpoints
{
    public static void MapShowEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/shows")
            .WithTags("TV Shows")
            .MapShowEndpoints();
    }

    private static RouteGroupBuilder MapShowEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/top", GetTopShows).WithSummary("Top Shows");
        group.MapGet("/{id}", GetShowDetails).WithSummary("Show Details");
        group.MapGet("/{showId}/season/{seasonNumber}", GetSeasonDetails).WithSummary("Season Details");
        group.MapGet("/{showId}/season/{seasonNumber}/episode/{episodeNumber}", GetEpisodeDetails).WithSummary("Episode Details");
        group.MapGet("/search", SearchShows).WithSummary("Search Shows");
        group.MapDelete("/cache", InvalidateCache).RequireAuthorization(policy => policy.RequireRole("Admin")).WithSummary("Clear Cache");
        group.MapPost("/", AddShow).RequireAuthorization(policy => policy.RequireRole("Admin")).WithSummary("Add Show (Optional)");
        group.MapPut("/{id}", UpdateShow).RequireAuthorization(policy => policy.RequireRole("Admin")).WithSummary("Update Show (Optional)");

        return group;
    }

    private static async Task<IResult> GetTopShows(ITMDbService tmdbService, [FromQuery] string? language = null,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        try
        {
            var shows = await tmdbService.GetTopShowsAsync(language!, page ?? 1, pageSize ?? 20);
            return Results.Ok(shows);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while fetching top shows: {ex.Message}");
        }
    }

    private static async Task<IResult> GetShowDetails(
        int id,
        ITMDbService tmdbService)
    {
        try
        {
            var show = await tmdbService.GetShowDetailsAsync(id);
            if (show == null)
            {
                return Results.NotFound($"Show with ID {id} not found.");
            }
            return Results.Ok(show);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while fetching show details: {ex.Message}");
        }
    }

    private static async Task<IResult> GetSeasonDetails(
            int showId,
            int seasonNumber,
            ITMDbService tmdbService)
    {
        try
        {
            var episode = await tmdbService.GetShowSeasonDetailsAsync(showId, seasonNumber);
            if (episode == null)
            {
                return Results.NotFound($"Season {seasonNumber} for show id {showId} not found.");
            }
            return Results.Ok(episode);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while fetching season details: {ex.Message}");
        }
    }

    private static async Task<IResult> GetEpisodeDetails(
            int showId,
            int seasonNumber,
            int episodeNumber,
            ITMDbService tmdbService)
    {
        try
        {
            var episode = await tmdbService.GetEpisodeDetailsAsync(showId, seasonNumber, episodeNumber);
            if (episode == null)
            {
                return Results.NotFound($"Episode {episodeNumber} season {seasonNumber} for show id {showId} not found.");
            }
            return Results.Ok(episode);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while fetching episode details: {ex.Message}");
        }
    }

    private static async Task<IResult> SearchShows(
        [FromQuery] string query,
        ITMDbService tmdbService)
    {
        try
        {
            var shows = await tmdbService.SearchShowsAsync(query);
            return Results.Ok(shows);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while searching for shows: {ex.Message}");
        }
    }

    // These two are optional, TMDb API is the primary data source 
    private static async Task<IResult> AddShow([FromBody] TVShow show, HttpContext context, ITVShowRepository showRepository)
    {
        if (!context.IsInRole("Admin"))
        {
            return Results.Forbid();
        }

        try
        {
            var showData = await showRepository.GetTopShowsAsync();

            if (showData == null || !showData.Any())
            {
                return Results.Problem("Cannot add shows manually as show data service is currently available");
            }

            await showRepository.AddOrUpdateAsync(show);
            return Results.Created($"/api/shows/{show.Id}", show);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while adding the show: {ex.Message}");
        }
    }

    private static async Task<IResult> UpdateShow(int id, [FromBody] TVShow updatedShow, HttpContext context, ITVShowRepository showRepository)
    {
        if (!context.IsInRole("Admin"))
        {
            return Results.Forbid();
        }

        try
        {
            var showData = await showRepository.GetTopShowsAsync();
            if (showData == null || !showData.Any())
            {
                return Results.Problem("Cannot add shows manually as show data service is currently available");
            }

            var existingShow = await showRepository.GetShowByIdAsync(id);
            if (existingShow == null)
            {
                return Results.NotFound($"Show with ID {id} not found.");
            }

            existingShow.Name = updatedShow.Name;
            existingShow.Overview = updatedShow.Overview;
            existingShow.FirstAirDate = updatedShow.FirstAirDate;
            existingShow.Popularity = updatedShow.Popularity;
            existingShow.PosterPath = updatedShow.PosterPath;
            existingShow.CreatedAt = updatedShow.CreatedAt;

            await showRepository.AddOrUpdateAsync(existingShow);

            return Results.Ok(existingShow);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while updating the show: {ex.Message}");
        }
    }

    private static async Task<IResult> InvalidateCache(string cachKey, ITMDbService tmdbService, HttpContext context)
    {
        if (!context.IsInRole("Admin"))
        {
            return Results.Forbid();
        }

        try
        {
            await tmdbService.InvalidateCacheAsync(cachKey);

            return Results.Created();
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while invalidating cache: {ex.Message}");

        }
    }
}