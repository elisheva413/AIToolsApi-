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
using Microsoft.Extensions.Logging;

namespace Tests
{
    public class OrderServiceUnitTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IKafkaProducerService> _kafkaProducerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Mock<ILogger<OrderService>> _loggerMock;

        public OrderServiceUnitTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productServiceMock = new Mock<IProductService>();
            _kafkaProducerMock = new Mock<IKafkaProducerService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_orderRepoMock.Object, _productServiceMock.Object, _kafkaProducerMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddOrder_HappyPath_CorrectSum_ReturnsOrderDTO()
        {
            // Arrange
            var productDto = new ProductDTO(1, 1, "Prod", "Desc", 50, "url1", "url2", "red", "gold", 10, true);

            var orderItemsDto = new List<OrderItemCreteDTO> { new OrderItemCreteDTO(1, 2) };
            var orderCreateDto = new OrderCreateDTO(new DateOnly(2024, 1, 1), 100, 1, orderItemsDto);

            var order = new Order
            {
                OrderId = 1,
                OrderSum = 100,
                UserId = 1,
                OrdersItems = new List<OrdersItem>
                {
                    new OrdersItem { ProductsId = 1, Quantity = 2 }
                }
            };

            _mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderCreateDTO>())).Returns(order);
            _productServiceMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productDto);
            _orderRepoMock.Setup(r => r.AddOrder(It.IsAny<Order>())).ReturnsAsync(order);
            _orderRepoMock.Setup(r => r.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<Order, OrderDTO>(It.IsAny<Order>()))
                       .Returns(new OrderDTO { OrderId = 1, OrderSum = 100 });

            // Act
            var result = await _orderService.AddOrder(orderCreateDto);

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

            var orderItemsDto = new List<OrderItemCreteDTO> { new OrderItemCreteDTO(1, 1) };
            var orderCreateDto = new OrderCreateDTO(new DateOnly(2024, 1, 1), 999, 1, orderItemsDto);

            var order = new Order
            {
                OrderSum = 999,
                UserId = 1,
                OrdersItems = new List<OrdersItem>
                {
                    new OrdersItem { ProductsId = 1, Quantity = 1 }
                }
            };

            _mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderCreateDTO>())).Returns(order);
            _productServiceMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productDto);

            // Act
            var result = await _orderService.AddOrder(orderCreateDto);

            // Assert
            Assert.Null(result);
            _orderRepoMock.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Never);
        }
    }
}