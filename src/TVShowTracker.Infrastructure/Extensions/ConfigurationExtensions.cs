using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace TVShowTracker.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(ApplicationDbContextSchema.SqlServerConnectionStringName)));

        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddDistributedMemoryCache();

        // Rate limiting configuration
        ConfigureRateLimiting(services, configuration);

        // TMDb API client configuration
        ConfigureTMDbClient(services, configuration);

        // JWT configuration
        ConfigureJwtAuthentication(services, configuration, environment);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                builder => builder.WithOrigins("http://localhost:5173")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        // Repository registrations
        services.AddScoped<ITVShowRepository, TVShowRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<IWatchedEpisodeRepository, WatchedEpisodeRepository>();

        // Service registrations
        services.AddScoped<ITMDbService, TMDbService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWatchedEpisodeService, WatchedEpisodeService>();

        return services;
    }

    private static void ConfigureRateLimiting(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

    private static void ConfigureTMDbClient(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TMDbOptions>(configuration.GetSection("TMDb"));
        services.AddHttpClient("TMDbApi", (sp, client) =>
        {
            var tmdbOptions = sp.GetRequiredService<IOptions<TMDbOptions>>().Value;
            client.BaseAddress = new Uri(tmdbOptions.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tmdbOptions.ReadAccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddScoped<ITMDbApiClient, TMDbApiClient>();
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

        if (string.IsNullOrEmpty(jwtOptions?.Key))
        {
            throw new InvalidOperationException("JWT Key is not configured.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireExpirationTime = true
            };

            options.RequireHttpsMetadata = !environment.IsDevelopment();

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    logger.LogError(context.Exception, "An error occurred during authentication");

                    context.NoResult();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { message = "Authentication failed", error = context.Exception.Message });
                    return context.Response.WriteAsync(result);
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { message = "You are not authorized" });
                    return context.Response.WriteAsync(result);
                }
            };
        });

        services.AddSingleton<IJwtService, JwtService>();
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}