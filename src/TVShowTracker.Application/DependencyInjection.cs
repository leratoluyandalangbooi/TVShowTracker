using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVShowTracker.Application.Services;

namespace TVShowTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IShowService, ShowService>();

        return services;
    }
}
