using WheaterRecBackend.Models;
using RestSharp;

namespace WheaterRecBackend.Services;

public class WeatherService
{
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public WeatherService(IConfiguration configuration)
    {
        _apiKey = configuration["WeatherStack:ApiKey"];
        _baseUrl = configuration["WeatherStack:BaseUrl"];
    }

    public async Task<WeatherData> GetCurrentWeatherAsync(string location)
    {
        // Construct the API request URL
        Console.WriteLine("api" + _apiKey);
        var client = new RestClient($"{_baseUrl}?access_key={_apiKey}&query={location}");
        var request = new RestRequest();
        request.Method = Method.Get;
        var response = await client.ExecuteAsync(request);

        // Log the raw response for troubleshooting
        Console.WriteLine($"API Response: {response.Content}");

        if (response.IsSuccessful)
        {
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(response.Content);

                // Check for an error object in the response
                if (json.RootElement.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetProperty("info").ToString();
                    Console.WriteLine($"Weather API returned an error: {errorMessage}");
                    return null;
                }

                // Check for the "current" property
                if (json.RootElement.TryGetProperty("current", out var currentElement))
                {
                    var temp = currentElement.GetProperty("temperature").ToString();
                    var desc = currentElement.GetProperty("weather_descriptions")[0].ToString();

                    return new WeatherData
                    {
                        Location = location,
                        Temperature = temp + "°C",
                        Description = desc,
                        Timestamp = DateTime.Now
                    };
                }

                Console.WriteLine("Unexpected response format: Missing 'current' property.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while parsing the API response: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Weather API request failed with status: {response.StatusCode}, Content: {response.Content}");
        }

        return null;
    }

}