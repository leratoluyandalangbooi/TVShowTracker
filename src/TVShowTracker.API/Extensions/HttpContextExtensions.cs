using System.Security.Claims;

namespace TVShowTracker.API.Extensions;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid User ID in token");
        }

        return userId;
    }

    public static string GetUsername(this HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Name)?.Value
            ?? throw new UnauthorizedAccessException("Username not found in token");
    }

    public static string GetUserEmail(this HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedAccessException("User email not found in token");
    }

    public static bool IsInRole(this HttpContext context, string role)
    {
        return context.User.IsInRole(role);
    }
}