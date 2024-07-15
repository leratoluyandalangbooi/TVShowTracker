namespace TVShowTracker.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(ApplicationDbContextSchema.SqlServerConnectionStringName));
            //options.UseSqlite(configuration.GetConnectionString(ApplicationDbContextSchema.SqlLiteConnectionStringName));
        });

        services.AddHttpClient<ITMDbApiClient, TMDbApiClient>();

        services.AddScoped<ITMDbService, TMDbService>();
        services.AddScoped<IShowRepository, ShowRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();

        services.AddScoped<ISearchService, ElasticSearchService>();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        return services;
    }

}
