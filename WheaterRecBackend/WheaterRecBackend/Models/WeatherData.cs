namespace WheaterRecBackend.Models;

public class WeatherData
{
    public string Location { get; set; }
    public string Temperature { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
}