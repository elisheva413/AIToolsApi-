using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositeries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.TestRepository.UnitTest
{
    public class OrderRepositoryUnitTests
    {
        [Fact]
        public async Task GetOrderById_ExistingOrder_ReturnsOrder()
        {
            // Arrange
            var orderId = 1;
            var orders = new List<Order>
            {
                new Order { OrderId = orderId, OrderSum = 250 }
            };

            var mockContext = new Mock<Store_215962135Context>(new DbContextOptions<Store_215962135Context>());
            mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);
            mockContext.Setup(ctx => ctx.Orders.FindAsync(orderId)).ReturnsAsync(orders[0]);

            var repository = new OrderRepository(mockContext.Object);

            // Act
            var result = await repository.GetOrderById(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(250, result.OrderSum);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsUserOrders()
        {
            // Arrange
            var userId = 5;
            var orders = new List<Order>
            {
                new Order { OrderId = 1, UserId = userId, OrderSum = 100 },
                new Order { OrderId = 2, UserId = 99, OrderSum = 200 }
            };

            var mockContext = new Mock<Store_215962135Context>(new DbContextOptions<Store_215962135Context>());
            mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);

            var repository = new OrderRepository(mockContext.Object);

            // Act
            var result = await repository.GetOrdersByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}