using Microsoft.AspNetCore.Mvc;
using WheaterRecBackend.Services;

namespace WheaterRecBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly KafkaProducerService _producerService;
        private readonly WeatherService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, KafkaProducerService producerService, WeatherService weatherService)
        {
            _logger = logger;
            _producerService = producerService;
            _weatherService = weatherService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentWeather([FromQuery] string query)
        {
            try
            {
                // Fetch weather data from Weatherstack API
                var weatherData = await _weatherService.GetWeatherAsync(query);

                // Produce the weather data to Kafka
                await _producerService.ProduceAsync(weatherData);

                return Ok(weatherData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        
    }
}
