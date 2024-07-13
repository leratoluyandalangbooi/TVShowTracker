namespace TVShowTracker.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(ApplicationDbContextSchema.DefaultConnectionStringName));
        });

        services.AddHttpClient<ITMDbService, TMDbService>();
        services.AddTransient<IShowRepository, ShowRepository>();
        services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }

}
