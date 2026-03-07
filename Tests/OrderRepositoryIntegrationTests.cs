using Entities;
using Repositeries;
using Repositeries.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class OrderRepositoryIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly Store_215962135Context _dbContext;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _orderRepository = new OrderRepository(_dbContext);
        }

        public async Task InitializeAsync()
        {
            await ClearDatabase();
        }

        public async Task DisposeAsync()
        {
            await ClearDatabase();
        }

        private async Task ClearDatabase()
        {
            _dbContext.ChangeTracker.Clear();

            // OrdersItems -> Orders -> Products -> Categories -> Users
            if (_dbContext.OrdersItems.Any())
                _dbContext.OrdersItems.RemoveRange(_dbContext.OrdersItems);

            if (_dbContext.Orders.Any())
                _dbContext.Orders.RemoveRange(_dbContext.Orders);

            if (_dbContext.Products.Any())
                _dbContext.Products.RemoveRange(_dbContext.Products);

            if (_dbContext.Categories.Any())
                _dbContext.Categories.RemoveRange(_dbContext.Categories);

            if (_dbContext.Users.Any())
                _dbContext.Users.RemoveRange(_dbContext.Users);

            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddOrder_SavesOrderAndItems()
        {
            // Arrange: Category
            var category = new Category
            {
                CategoryName = "Electronics"
            };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();


            var product1 = new Product
            {
                ProductsName = "Product 1",
                ProductsDescreption = "Desc 1",
                Price = 10,
                ImgUrl = "order_p1.jpg",
                CategoryId = category.CategoryId
            };

            var product2 = new Product
            {
                ProductsName = "Product 2",
                ProductsDescreption = "Desc 2",
                Price = 15,
                ImgUrl = "order_p2.jpg",
                CategoryId = category.CategoryId
            };

            await _dbContext.Products.AddRangeAsync(product1, product2);
            await _dbContext.SaveChangesAsync();
            var user = new User
            {
                UserName = "user1",
                Password = "StrongPass123!",
                FirstName = "Test",
                LastName = "User"
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();


            var order = new Order
            {

                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderSum = 35, 
                UserId = user.UserId,
                OrdersItems =
                {
                    new OrdersItem { ProductsId = product1.ProductsId, Quantity = 2 },
                    new OrdersItem { ProductsId = product2.ProductsId, Quantity = 1 }
                }
            };

            // Act
            var result = await _orderRepository.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.OrderId > 0);
            Assert.Equal((int)35, result.OrderSum);

            var savedItemsCount = _dbContext.OrdersItems.Count(oi => oi.OrderId == result.OrderId);
            Assert.Equal(2, savedItemsCount);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenExists()
        {
            // Arrange: User
            var user = new User
            {
                UserName = "test_user_2",
                Password = "StrongPass123!",
                FirstName = "T2",
                LastName = "U2"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderSum = 20,
                UserId = user.UserId
            };
            await _orderRepository.AddOrder(order);

            // Act
            var result = await _orderRepository.GetOrderById(order.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Equal((int)20, result.OrderSum);
            Assert.Equal(user.UserId, result.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _orderRepository.GetOrderById(9999);

            // Assert
            Assert.Null(result);
        }
    }
}
