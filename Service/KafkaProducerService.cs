using Confluent.Kafka;
using DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly string _orderCreatedTopic;
        private readonly ProducerConfig _producerConfig;

        public KafkaProducerService(IConfiguration configuration)
        {
            string bootstrapServers = configuration["Kafka:BootstrapServers"]
                ?? throw new InvalidOperationException("Kafka bootstrap servers are missing in configuration.");

            _orderCreatedTopic = configuration["Kafka:OrderCreatedTopic"]
                ?? throw new InvalidOperationException("Kafka order created topic is missing in configuration.");

            _producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
        }

        public async Task PublishOrderCreatedAsync(OrderCreatedEventDTO orderCreatedEvent)
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();

            string payload = JsonSerializer.Serialize(orderCreatedEvent);
            string key = orderCreatedEvent.OrderId.ToString();

            await producer.ProduceAsync(_orderCreatedTopic, new Message<string, string>
            {
                Key = key,
                Value = payload
            });
        }
    }
}
