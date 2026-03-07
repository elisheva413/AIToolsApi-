using Entities;
using Moq;
using Moq.EntityFrameworkCore;
using Repositeries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class OrderRepositoryUnitTests
    {
        [Fact]
        public async Task AddOrder_ReturnsOrder()
        {
            // Arrange
            var mockContext = new Mock<Store_215962135Context>();

            var orders = new List<Order>
            {
                new Order { OrderId = 1, OrderDate = DateOnly.FromDateTime(DateTime.Now), OrderSum = 250 }
            };

            var newOrder = new Order
            {
                OrderId = 2,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderSum = 200,
                UserId = 1
            };

            mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.AddOrder(newOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)200, result.OrderSum);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderSum = 250,
                UserId = 1
            };

            var orders = new List<Order> { order };

            var mockContext = new Mock<Store_215962135Context>();
            mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrderById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)250, result.OrderSum);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull()
        {
            // Arrange
            var orders = new List<Order>();

            var mockContext = new Mock<Store_215962135Context>();
            mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrderById(999);

            // Assert
            Assert.Null(result);
        }
    }
}
