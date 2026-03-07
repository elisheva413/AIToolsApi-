using Entities;
using Microsoft.EntityFrameworkCore;
using Repositeries;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Threading.Tasks;


namespace Repositeries
{
    public class OrderRepository : IOrderRepository
    {
        Store_215962135Context _store_215962135Context;
        public OrderRepository(Store_215962135Context store_215962135Context)
        {
            _store_215962135Context = store_215962135Context;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _store_215962135Context.Orders.FindAsync(id);
        }

        public async Task<Order> AddOrder(Order order)
        {
            await _store_215962135Context.Orders.AddAsync(order);
            await _store_215962135Context.SaveChangesAsync();
            return order;
            
        }






    }
}
