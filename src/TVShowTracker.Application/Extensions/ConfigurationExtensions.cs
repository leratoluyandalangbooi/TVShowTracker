namespace TVShowTracker.Application.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWatchlistService, WatchlistService>();
        return services;
    }
}
