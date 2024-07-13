using TVShowTracker.Application.Interfaces;
using TVShowTracker.Application;
using TVShowTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.ConfigureInfrastructureLayer(builder.Configuration);
builder.Services.ConfigureApplicationLayer(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/top", async (ITMDbService tmDbService) =>
{
    var results = await tmDbService.GetTopShowsAsync();
    return Results.Ok(results);
});

app.MapGet("/", () => "Hello World!");

app.Run();
