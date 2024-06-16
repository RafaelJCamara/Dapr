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
        const string DAPR_CONFIGURATION_STORE = "configstore";
        var CONFIGURATION_ITEMS = new List<string> { "id1" };
        var sentItems = new List<string>();

        using var client = new DaprClientBuilder().Build();

        GetConfigurationResponse config = await client.GetConfiguration(DAPR_CONFIGURATION_STORE, CONFIGURATION_ITEMS);
        foreach (var item in config.Items)
        {
            var conf = item.Value.Value;
            
            Console.WriteLine("Configuration for " + item.Key + ": " + conf);
            sentItems.Add(conf);

            // Publish an event/message using Dapr PubSub
            await client.PublishEventAsync("pubsub", "orders", new Order(Int64.Parse(conf)));
        }

        return $"message sent to app 2. number sent: {string.Join(", ", sentItems)}";
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

public record Order([property: JsonPropertyName("orderId")] double OrderId);