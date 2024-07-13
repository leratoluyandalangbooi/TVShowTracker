using Microsoft.Extensions.DependencyInjection;
using TVShowTracker.Infrastructure.Persistence;
using TVShowTracker.Infrastructure.Persistence.Repositories;
using TVShowTracker.Infrastructure.Services.Externals;

namespace TVShowTracker.Infrastructure;

public static class DependencyInjenction
{
    public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("tvshowtracker"));
        });

        services.AddHttpClient<ITMDbService, TMDbService>();
        services.AddTransient<IShowRepository, ShowRepository>();
        //services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }

}
