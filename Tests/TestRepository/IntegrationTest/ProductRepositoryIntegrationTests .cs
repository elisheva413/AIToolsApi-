using Entities;
using Repositeries;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Tests.TestRepository.IntegrationTest
{
    [Collection("Database Collection")]
    public class ProductRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly Store_215962135Context _dbContext;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _productRepository = new ProductRepository(_dbContext);
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
            if (_dbContext.Products.Any())
                _dbContext.Products.RemoveRange(_dbContext.Products);
            if (_dbContext.Categories.Any())
                _dbContext.Categories.RemoveRange(_dbContext.Categories);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProducts_NoFilters_ReturnsAll()
        {
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Products.AddRangeAsync(
                new Product { ProductsName = "Laptop", ProductsDescreption = "High performance", Price = 1200, CategoryId = category.CategoryId, ImgUrl = "1.jpg", Quantity = 10, IsActive = true },
                new Product { ProductsName = "Smartphone", ProductsDescreption = "Latest model", Price = 800, CategoryId = category.CategoryId, ImgUrl = "2.jpg", Quantity = 5, IsActive = true }
            );
            await _dbContext.SaveChangesAsync();

            var (items, total) = await _productRepository.GetProducts(
                categoryId: null, q: null, minPrice: null, maxPrice: null, color: null, material: null, inStock: null, isActive: true, sort: null, skip: 10, position: 1
            );

            Assert.Equal(2, total);
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public async Task GetProducts_FilterBySearchTerm_ReturnsMatching()
        {
            var category = new Category { CategoryName = "Tech" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Products.AddAsync(new Product { ProductsName = "Smartwatch", ProductsDescreption = "Wearable tech", Price = 200, CategoryId = category.CategoryId, ImgUrl = "3.jpg", IsActive = true });
            await _dbContext.SaveChangesAsync();

            var (items, total) = await _productRepository.GetProducts(
                categoryId: null, q: "smart", minPrice: null, maxPrice: null, color: null, material: null, inStock: null, isActive: true, sort: null, skip: 10, position: 1
            );

            Assert.Single(items);
            Assert.Equal("Smartwatch", items.First().ProductsName);
        }
    }
}