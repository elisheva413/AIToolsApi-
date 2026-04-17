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
            using (var transaction = await _store_215962135Context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _store_215962135Context.Orders.AddAsync(order);
                    await _store_215962135Context.SaveChangesAsync();

                    if (order.OrdersItems != null)
                    {
                        foreach (var item in order.OrdersItems)
                        {
                            var product = await _store_215962135Context.Products.FindAsync(item.ProductsId);
                            if (product != null)
                            {
                                product.Quantity -= item.Quantity;

                                if (product.Quantity <= 0)
                                {
                                    product.Quantity = 0; 
                                    product.IsActive = false;
                                }
                            }
                        }
                        await _store_215962135Context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return order;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _store_215962135Context.Orders
                .Include(o => o.OrdersItems)
                    .ThenInclude(oi => oi.Products) 
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            return await _store_215962135Context.Orders
                .Include(o => o.OrdersItems)
                    .ThenInclude(oi => oi.Products) 
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order?> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _store_215962135Context.Orders.FindAsync(orderId);

            if (order != null)
            {
                order.OrderStatus = newStatus;
                await _store_215962135Context.SaveChangesAsync();
            }

            return order;
        }






    }
}
