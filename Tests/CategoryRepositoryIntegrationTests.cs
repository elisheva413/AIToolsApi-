using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositeries;

using Xunit;


namespace Tests
{
    public class CategoryRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly Store_215962135Context _dbContext;
        private readonly CategoryRepository _categoryRepository;

        public CategoryRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [Fact]
        public async Task GetCategories_ValidCategories_ReturnsCategories()
        {
            // Arrange
            var Category1 = new Category { CategoryName = "Electronics" };
            var Category2 = new Category { CategoryName = "Books" };
            _dbContext.Categories.Add(Category1);
            _dbContext.Categories.Add(Category2);
            await _dbContext.SaveChangesAsync();



            // Act
            var result = await _categoryRepository.GetCategory();
 
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Electronics");
            Assert.Contains(result, c => c.CategoryName == "Books");

        }
    }
}

