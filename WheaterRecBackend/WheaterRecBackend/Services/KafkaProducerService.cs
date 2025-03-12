using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using WheaterRecBackend.Models;

namespace WheaterRecBackend.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:Topic"];
        }

        public async Task SendWeatherDataAsync(WeatherData weatherData)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            try
            {
                // Convert WeatherData object to JSON string
                var message = JsonSerializer.Serialize(weatherData);

                // Produce message to Kafka
                var deliveryResult = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });

                Console.WriteLine($"Message sent to Kafka topic '{_topic}': {message}");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
            }
        }
    }
}
