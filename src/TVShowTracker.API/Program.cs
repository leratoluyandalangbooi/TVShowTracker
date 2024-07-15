using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TVShowTracker.API.Endpoints;
using TVShowTracker.API.Middleware;
using TVShowTracker.Application.Extensions;
using TVShowTracker.Infrastructure.Extensions;
using TVShowTracker.Infrastructure.Persistence;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.ConfigureInfrastructureLayer(builder.Configuration);
builder.Services.ConfigureApplicationLayer(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    //.AddTransforms(transforms =>
    //{
    //    transforms.AddRequestTransform(transform =>
    //    {
    //        transform.RequestHeaders.Remove("Authorization");
    //        return ValueTask.CompletedTask;
    //    });
    //})
    ;

//builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
//    .AddBearerToken();

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
//});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

//app.UseMiddleware<JwtAuthenticationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapShowEndpoints();
app.MapWatchlistEndpoints();
app.MapSearchEndpoints();

app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

//app.MapReverseProxy();

app.Run();
