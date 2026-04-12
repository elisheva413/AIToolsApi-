using DTOs;
using Entities;

namespace Service
{
    public interface IOrderService
    {
        Task<OrderDTO> AddOrder(Order order);
        Task<OrderDTO?> GetOrderById(int id);

        Task<IEnumerable<OrderDTO>> GetAllOrders();
        Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId);
        Task<OrderDTO?> UpdateOrderStatus(int orderId, string newStatus);



    }
}
