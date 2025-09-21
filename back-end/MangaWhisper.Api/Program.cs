using MangaWhisper.Infrastructure.Extensions;
using MangaWhisper.Application.Extensions;
using DotNetEnv;

// Load .env file
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://manga-whisper-production.web.app"
                )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Register services by layer
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowSpecificOrigins");
}
else
{
    app.UseCors("AllowSpecificOrigins");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
