using System.Threading.Tasks;
using Entities;
using Repositeries;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Tests.TestRepository.IntegrationTest
{
    [Collection("Database Collection")]
    public class CategoryRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly Store_215962135Context _dbContext;
        private readonly CategoryRepository _categoryRepository;

        public CategoryRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        public async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await ClearDatabaseAsync();
        }

        private async Task ClearDatabaseAsync()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetCategory_ValidCategories_ReturnsCategories()
        {
            var category1 = new Category { CategoryName = "Electronics" };
            var category2 = new Category { CategoryName = "Books" };

            await _dbContext.Categories.AddRangeAsync(category1, category2);
            await _dbContext.SaveChangesAsync();

            var result = await _categoryRepository.GetCategory();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Electronics");
        }
    }
}