using Microsoft.AspNetCore.Mvc;
using TVShowTracker.API.Extensions;
using TVShowTracker.Application.Abstractions;
using TVShowTracker.Application.Interfaces;
using TVShowTracker.Domain.Entities;

namespace TVShowTracker.API.Endpoints;

public static class ShowEndpoints
{
    public static void MapShowEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/shows")
            .WithTags("Shows")
            .MapShowEndpoints();
    }

    private static RouteGroupBuilder MapShowEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/top", GetTopShows);
        group.MapGet("/{id}", GetShowDetails);
        group.MapGet("/{showId}/season/{seasonNumber}/episode/{episodeNumber}", GetEpisodeDetails);
        group.MapGet("/search", SearchShows);
        group.MapPost("/", AddShow).RequireAuthorization("AdminPolicy");
        group.MapPut("/{id}", UpdateShow).RequireAuthorization("AdminPolicy");

        return group;
    }

    private static async Task<IResult> GetTopShows([FromQuery] string language,
        [FromQuery] int page, [FromQuery] int pageSize,
        ITMDbService tmdbService)
    {
        try
        {
            var shows = await tmdbService.GetTopShowsAsync(language, page, pageSize);
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
                return Results.NotFound($"Episode not found for Show ID {showId}, Season {seasonNumber}, Episode {episodeNumber}.");
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
    
    //these two are now optional, as TMDb API is the primary data source
    //move it to admin setups
    private static async Task<IResult> AddShow([FromBody] Show show, HttpContext context, IShowRepository showRepository)
    {
        if (!context.IsInRole("Admin"))
        {
            return Results.Forbid();
        }

        try
        {
            await showRepository.AddOrUpdateAsync(show);
            return Results.Created($"/api/shows/{show.Id}", show);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while adding the show: {ex.Message}");
        }
    }

    private static async Task<IResult> UpdateShow( int id, [FromBody] Show updatedShow, HttpContext context, IShowRepository showRepository)
    {
        if (!context.IsInRole("Admin"))
        {
            return Results.Forbid();
        }

        try
        {
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

            await showRepository.AddOrUpdateAsync(existingShow);

            return Results.Ok(existingShow);
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred while updating the show: {ex.Message}");
        }
    }
}
