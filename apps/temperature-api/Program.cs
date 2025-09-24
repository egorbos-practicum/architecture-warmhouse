using System.Globalization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/temperature", ([FromQuery] string? location) =>
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return new TemperatureResponse("Unknown", 0);
        }
        var sensorLocation = int.TryParse(location, out var locationId)
            ? locationId switch
            {
                1 => "Living Room",
                2 => "Bedroom",
                3 => "Kitchen",
                _ => "Unknown"
            } : location;
        return new TemperatureResponse(sensorLocation, 0);
    })
.WithName("GetTemperatureByLocation")
.WithOpenApi();

app.MapGet("/temperature/{sensorId}", (int sensorId) => new TemperatureResponse("", sensorId))
.WithName("GetTemperatureBySensorId")
.WithOpenApi();

app.MapGet("/health", () => "Healthy")
.WithName("Healthcheck")
.WithOpenApi();

app.Run();

internal static class Data
{
    public static readonly string[] Statuses = ["Healthy", "Unhealthy"];
    public static readonly HashSet<string> Locations = ["living room", "bedroom", "kitchen", "unknown"];
}

internal record TemperatureResponse
{
    public double Value { get; set; }
    public string Unit { get; set; }
    public DateTime Timestamp { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public string SensorID { get; set; }
    public string SensorType { get; set; }
    public string Description { get; set; }

    public TemperatureResponse(string location, int sensorId)
    {
        Value = Random.Shared.Next(18, 29);
        Unit = "°C";
        Timestamp = DateTime.Now;
        Location = Data.Locations.Contains(location.ToLower())
            ? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(location)
            : sensorId switch
            {
                1 => "Living Room",
                2 => "Bedroom",
                3 => "Kitchen",
                _ => "Unknown"
            };
        Status = Data.Statuses[Random.Shared.Next(Data.Statuses.Length)];
        SensorID = Location.ToLower() switch
        {
            "living room" => "1",
            "bedroom" => "2",
            "kitchen" => "3",
            _ => "0"
        };
        SensorType = "Sensor";
        Description = "Description";
    }
}