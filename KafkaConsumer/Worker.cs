namespace KafkaConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string bootstrapServers = _configuration["Kafka:BootstrapServers"]
            ?? throw new InvalidOperationException("Kafka bootstrap servers are missing in configuration.");

        string topic = _configuration["Kafka:OrderCreatedTopic"]
            ?? throw new InvalidOperationException("Kafka order created topic is missing in configuration.");

        string groupId = _configuration["Kafka:GroupId"] ?? "webapishop-order-consumer";

        var consumerConfig = new Confluent.Kafka.ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new Confluent.Kafka.ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);
        _logger.LogInformation("Kafka consumer started. Topic: {Topic}, GroupId: {GroupId}", topic, groupId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult?.Message != null)
                {
                    _logger.LogInformation(
                        "Kafka message received. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}, Value: {Value}",
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        consumeResult.Message.Key,
                        consumeResult.Message.Value);

                    // Manual commit after successful processing keeps lag metrics meaningful in Kafka UI.
                    consumer.Commit(consumeResult);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consume loop error.");
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer stopped.");
    }
}
