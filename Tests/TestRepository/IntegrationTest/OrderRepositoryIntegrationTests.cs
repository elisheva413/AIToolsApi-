using Entities;
using Repositeries;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.TestRepository.IntegrationTest
{
    [Collection("Database Collection")]
    public class OrderRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly Store_215962135Context _dbContext;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _orderRepository = new OrderRepository(_dbContext);
        }

        public async Task InitializeAsync() => await ClearDatabase();
        public async Task DisposeAsync() => await ClearDatabase();

        private async Task ClearDatabase()
        {
            _dbContext.ChangeTracker.Clear();
            if (_dbContext.OrdersItems.Any()) _dbContext.OrdersItems.RemoveRange(_dbContext.OrdersItems);
            if (_dbContext.Orders.Any()) _dbContext.Orders.RemoveRange(_dbContext.Orders);
            if (_dbContext.Products.Any()) _dbContext.Products.RemoveRange(_dbContext.Products);
            if (_dbContext.Users.Any()) _dbContext.Users.RemoveRange(_dbContext.Users);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddOrder_Integration_SavesToDatabase()
        {
            // Arrange
            var user = new User { UserName = "order@test.com", Password = "123", FirstName = "A", LastName = "B" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.UserId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderSum = 150,
                OrderStatus = "New"
            };

            // Act
            var result = await _orderRepository.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.OrderId > 0);

            var savedOrder = await _dbContext.Orders.FindAsync(result.OrderId);
            Assert.NotNull(savedOrder);
            Assert.Equal(150, savedOrder.OrderSum);
        }
    }
}