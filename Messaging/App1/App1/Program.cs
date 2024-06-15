using System.Text.Json.Serialization;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async () =>
    {
        using var client = new DaprClientBuilder().Build();
        
        // Publish an event/message using Dapr PubSub
        var rand = new Random().Next(0, 10);
        
        await client.PublishEventAsync("pubsub", "orders", new Order(rand));

        return $"message sent to app 2. number sent: {rand}";
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

public record Order([property: JsonPropertyName("orderId")] double OrderId);