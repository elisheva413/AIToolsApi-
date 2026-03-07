using DTOs;
using Entities;

namespace Service
{
    public interface IOrderService
    {
        Task<OrderDTO> AddOrder(Order order);
        Task<OrderDTO?> GetOrderById(int id);
    }
}
