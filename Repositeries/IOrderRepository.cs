using Entities;

namespace Repositeries
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order order);
        Task<Order?> GetOrderById(int id);

    }
}