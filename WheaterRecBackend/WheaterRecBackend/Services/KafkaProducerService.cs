using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using WheaterRecBackend.Models;

namespace WheaterRecBackend.Services
{
    using Confluent.Kafka;
    using Microsoft.Extensions.Configuration;

    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:Topic"];
        }

        public async Task ProduceAsync(string message)
        {
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
        }
    }
}
