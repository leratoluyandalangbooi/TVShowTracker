namespace TVShowTracker.Application.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IShowService, ShowService>();

        return services;
    }
}
