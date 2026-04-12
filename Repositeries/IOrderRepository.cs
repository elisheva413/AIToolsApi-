using Entities;

namespace Repositeries
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<Order?> UpdateOrderStatus(int orderId, string newStatus);


    }
}