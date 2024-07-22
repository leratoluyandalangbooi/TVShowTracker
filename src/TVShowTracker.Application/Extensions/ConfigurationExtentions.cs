using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TVShowTracker.Application.DTOs.Request;

namespace TVShowTracker.Application.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IValidator<GetShowDetailsDto>, GetShowDetailsDtoValidator>();
        services.AddScoped<IValidator<GetEpisodeDetailsDto>, GetEpisodeDetailsDtoValidator>();
        services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();

        return services;
    }
}