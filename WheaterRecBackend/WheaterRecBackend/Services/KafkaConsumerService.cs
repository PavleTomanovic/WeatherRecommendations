using Confluent.Kafka;
using Newtonsoft.Json;

namespace WheaterRecBackend.Services;


public class KafkaConsumerService : IHostedService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;

    public KafkaConsumerService(IConfiguration configuration)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _topic = configuration["Kafka:Topic"];
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }
}