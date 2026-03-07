using Entities;
using Repositeries;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ProductRepositoryIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
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
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Products.AddRangeAsync(
                new Product { ProductsName = "Laptop", ProductsDescreption = "High performance", Price = 1200, CategoryId = category.CategoryId, ImgUrl = "img_laptop_1.jpg" },
                new Product { ProductsName = "Smartphone", ProductsDescreption = "Latest model", Price = 800, CategoryId = category.CategoryId, ImgUrl = "img_phone_1.jpg" },
                new Product { ProductsName = "Headphones", ProductsDescreption = "Noise cancelling", Price = 100, CategoryId = category.CategoryId, ImgUrl = "img_head_1.jpg" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _productRepository.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: new int[] { },  
                position: 1,
                skip: 10
            );

            // Assert
            Assert.NotNull(items);
            Assert.Equal(3, items.Count);
            Assert.Equal(3, totalCount);
        }

        [Fact]
        public async Task GetProducts_FilterByDescription_ReturnsOnlyMatching()
        {
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Products.AddRangeAsync(
                new Product { ProductsName = "Laptop", ProductsDescreption = "High performance laptop", Price = 1200, CategoryId = category.CategoryId, ImgUrl = "img_laptop_2.jpg" },
                new Product { ProductsName = "Smartphone", ProductsDescreption = "Latest model smartphone", Price = 800, CategoryId = category.CategoryId, ImgUrl = "img_phone_2.jpg" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _productRepository.GetProducts(
                description: "smart",
                minPrice: 50,
                maxPrice: 1000,
                categoriesId: new int[] { category.CategoryId },
                position: 1,
                skip: 10
            );

            // Assert
            Assert.Single(items);
            Assert.Equal(1, totalCount);
            Assert.Equal("Smartphone", items.First().ProductsName);
        }

        [Fact]
        public async Task GetProducts_NoProductsFound_ReturnsEmpty()
        {
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _productRepository.GetProducts(
                description: "NonExisting",
                minPrice: 1000,
                maxPrice: 2000,
                categoriesId: new int[] { category.CategoryId },
                position: 1,
                skip: 10
            );

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
            Assert.Equal(0, totalCount);
        }

        [Fact]
        public async Task GetProducts_EmptyCategoryIds_DoesNotFilterByCategory()
        {
            // Arrange
            var category1 = new Category { CategoryName = "Toys" };
            var category2 = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddRangeAsync(category1, category2);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Products.AddRangeAsync(
                new Product { ProductsName = "Doll", ProductsDescreption = "Kids toy", Price = 50, CategoryId = category1.CategoryId, ImgUrl = "img1.jpg" },
                new Product { ProductsName = "Laptop", ProductsDescreption = "Work device", Price = 1200, CategoryId = category2.CategoryId, ImgUrl = "img3.jpg" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _productRepository.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: new int[] { },
                position: 1,
                skip: 10
            );

            // Assert
            Assert.Equal(2, items.Count);
            Assert.Equal(2, totalCount);
        }

        [Fact]
        public async Task GetProducts_Paging_ReturnsCorrectPage()
        {
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // 3 מוצרים
            await _dbContext.Products.AddRangeAsync(
                new Product { ProductsName = "A", ProductsDescreption = "d", Price = 10, CategoryId = category.CategoryId, ImgUrl = "img_a.jpg" },
                new Product { ProductsName = "B", ProductsDescreption = "d", Price = 20, CategoryId = category.CategoryId, ImgUrl = "img_b.jpg" },
                new Product { ProductsName = "C", ProductsDescreption = "d", Price = 30, CategoryId = category.CategoryId, ImgUrl = "img_c.jpg" }
            );
            await _dbContext.SaveChangesAsync();

      
            var (items, totalCount) = await _productRepository.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: new int[] { category.CategoryId },
                position: 2,
                skip: 2
            );

            // Assert
            Assert.Equal(3, totalCount);
            Assert.Single(items);
        }
    }
}
