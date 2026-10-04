using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Temperature API",
        Version = "v1",
        Description = "Returns a random temperature reading for a smart home sensor"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck");

app.MapGet("/temperature", (string? location, string? sensorId) =>
        Results.Ok(TemperatureReading.Create(location, sensorId)))
    .WithName("GetTemperatureByLocation");

app.MapGet("/temperature/{sensorId}", (string sensorId) =>
        Results.Ok(TemperatureReading.Create(null, sensorId)))
    .WithName("GetTemperatureBySensorId");

app.Run();

public record TemperatureReading(
    [property: JsonPropertyName("value")] double Value,
    [property: JsonPropertyName("unit")] string Unit,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("sensor_id")] string SensorId,
    [property: JsonPropertyName("sensor_type")] string SensorType,
    [property: JsonPropertyName("description")] string Description)
{
    private static readonly Dictionary<string, string> LocationBySensorId = new()
    {
        ["1"] = "Living Room",
        ["2"] = "Bedroom",
        ["3"] = "Kitchen"
    };

    private static readonly Dictionary<string, string> SensorIdByLocation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Living Room"] = "1",
            ["Bedroom"] = "2",
            ["Kitchen"] = "3"
        };

    public static TemperatureReading Create(string? location, string? sensorId)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            location = LocationBySensorId.TryGetValue(sensorId ?? string.Empty, out var knownLocation)
                ? knownLocation
                : "Unknown";
        }

        if (string.IsNullOrWhiteSpace(sensorId))
        {
            sensorId = SensorIdByLocation.TryGetValue(location, out var knownSensorId)
                ? knownSensorId
                : "0";
        }

        var value = Math.Round(Random.Shared.NextDouble() * 15.0 + 15.0, 1);

        return new TemperatureReading(
            value,
            "°C",
            DateTime.UtcNow,
            location,
            "active",
            sensorId,
            "temperature",
            $"Temperature reading for {location}");
    }
}
