using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositeries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.TestRepository.UnitTest
{
    public class CategoryRepositoryUnitTests
    {
        [Fact]
        public async Task GetCategory_ReturnsListOfCategories()
        {
            var mockContext = new Mock<Store_215962135Context>(new DbContextOptions<Store_215962135Context>());

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Electronics" },
                new Category { CategoryId = 2, CategoryName = "Books" }
            };

            mockContext.Setup(c => c.Categories).ReturnsDbSet(categories);

            var repository = new CategoryRepository(mockContext.Object);

            var result = await repository.GetCategory();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Electronics");
            Assert.Contains(result, c => c.CategoryName == "Books");
        }
    }
}