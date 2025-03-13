using WheaterRecBackend.Models;
using RestSharp;

namespace WheaterRecBackend.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Weatherstack:ApiKey"];
        _baseUrl = configuration["Weatherstack:BaseUrl"];
    }

    public async Task<string> GetWeatherAsync(string query)
    {
        var requestUrl = $"{_baseUrl}?access_key={_apiKey}&query={query}";
        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw new HttpRequestException($"Failed to fetch weather data. Status code: {response.StatusCode}");
    }
}