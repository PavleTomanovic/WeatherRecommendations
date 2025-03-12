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
        
        [HttpGet("{location}")]
        public async Task<IActionResult> GetWeather(string location)
        {
            var weatherData = await _weatherService.GetCurrentWeatherAsync(location);
            if (weatherData == null)
            {
                return NotFound("Weather data could not be retrieved.");
            }

            // Send to Kafka
            await _producerService.SendWeatherDataAsync(weatherData);

            return Ok(weatherData);
        }
        
        
        
    }
}
