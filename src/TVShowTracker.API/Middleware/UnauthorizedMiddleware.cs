using System.Text.Json;

namespace TVShowTracker.API.Middleware
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = new { message = "Unauthorized: Invalid or missing authentication token" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
