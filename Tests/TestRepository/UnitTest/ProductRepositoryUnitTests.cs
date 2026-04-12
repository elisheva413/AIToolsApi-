using Entities;
using Repositeries;
using Moq;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Tests.TestRepository.UnitTest
{
    public class ProductRepositoryUnitTests
    {
        private Mock<Store_215962135Context> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<Store_215962135Context>().Options;
            return new Mock<Store_215962135Context>(options);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenDoesNotExist_ReturnsNull()
        {
            // Arrange
            var mockContext = GetMockContext();
            var products = new List<Product>();
            mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            mockContext.Setup(ctx => ctx.Products.FindAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = await repository.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenExists_ReturnsProduct()
        {
            // Arrange
            var targetId = 5;
            var products = new List<Product>
            {
                new Product { ProductsId = targetId, ProductsName = "Target Product" }
            };

            var mockContext = GetMockContext();
            mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            mockContext.Setup(ctx => ctx.Products.FindAsync(targetId)).ReturnsAsync(products[0]);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = await repository.GetProductByIdAsync(targetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Target Product", result.ProductsName);
        }

        [Fact]
        public async Task AddProduct_ValidData_CallsAddAndSave()
        {
            // Arrange
            var mockContext = GetMockContext();
            var products = new List<Product>();
            mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);

            var repository = new ProductRepository(mockContext.Object);
            var newProduct = new Product { ProductsName = "New Jewelry", Price = 450 };

            // Act
            var result = await repository.AddProduct(newProduct);

            // Assert
            Assert.NotNull(result);
            mockContext.Verify(x => x.Products.AddAsync(It.IsAny<Product>(), default), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}