using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TVShowTracker.API.Endpoints;
using TVShowTracker.API.Middleware;
using TVShowTracker.Application.Extensions;
using TVShowTracker.Infrastructure.Extensions;
using TVShowTracker.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

builder.Services.ConfigureInfrastructureLayer(builder.Configuration, builder.Environment);
builder.Services.ConfigureApplicationLayer(builder.Configuration);

//Add Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => {
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
else
{
    app.UseHttpsRedirection();
}

// Use middleware
app.UseCors("AllowReactApp");
app.UseIpRateLimiting();
app.UseMiddleware<JwtAuthenticationMiddleware>();
app.UseMiddleware<UnauthorizedMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapShowEndpoints();
app.MapWatchedEpisodeEndpoints();
app.MapUserEndpoints();

app.Run();