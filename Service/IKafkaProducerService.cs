using DTOs;
using System.Threading.Tasks;

namespace Service
{
    public interface IKafkaProducerService
    {
        Task PublishOrderCreatedAsync(OrderCreatedEventDTO orderCreatedEvent);
    }
}
