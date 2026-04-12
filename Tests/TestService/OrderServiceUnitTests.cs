using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Repositeries;
using Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class OrderServiceUnitTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;

        public OrderServiceUnitTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productServiceMock = new Mock<IProductService>();
            _mapperMock = new Mock<IMapper>();
            _orderService = new OrderService(_orderRepoMock.Object, _productServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddOrder_HappyPath_CorrectSum_ReturnsOrderDTO()
        {
            // Arrange
            var productDto = new ProductDTO(1, 1, "Prod", "Desc", 50, "url1", "url2", "red", "gold", 10, true);
            var order = new Order
            {
                OrderId = 1,
                OrderSum = 100,
                OrdersItems = new List<OrdersItem>
                {
                    new OrdersItem { ProductsId = 1, Quantity = 2 }
                }
            };

            _productServiceMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productDto);
            _orderRepoMock.Setup(r => r.AddOrder(It.IsAny<Order>())).ReturnsAsync(order);
            _orderRepoMock.Setup(r => r.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<Order, OrderDTO>(It.IsAny<Order>()))
                       .Returns(new OrderDTO { OrderId = 1, OrderSum = 100 });

            // Act
            var result = await _orderService.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.OrderSum);
            _orderRepoMock.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task AddOrder_UnhappyPath_IncorrectSum_ThrowsException()
        {
            // Arrange
            var productDto = new ProductDTO(1, 1, "Prod", "Desc", 50, "url1", "url2", "red", "gold", 10, true);
            var order = new Order
            {
                OrderSum = 999,
                OrdersItems = new List<OrdersItem>
                {
                    new OrdersItem { ProductsId = 1, Quantity = 1 }
                }
            };

            _productServiceMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productDto);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderService.AddOrder(order));

            Assert.NotNull(exception);
            Assert.Equal("Payment error", exception.Message);
            _orderRepoMock.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Never);
        }
    }
}