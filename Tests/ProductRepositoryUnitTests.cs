using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Entities;
using Repositeries;
using Moq.EntityFrameworkCore;

namespace Tests
{
    public class ProductRepositoryUnitTests
    {
        private static ProductRepository CreateRepo(List<Product> products)
        {
            var mockContext = new Mock<Store_215962135Context>();

            mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);

            return new ProductRepository(mockContext.Object);
        }

        [Fact]
        public async Task GetProducts_NoFilters_ReturnsAllAndTotalCount()
        {
            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductsName = "Laptop", ProductsDescreption = "High performance", Price = 1200, CategoryId = 100, ImgUrl="1.jpg" },
                new Product { ProductsId = 2, ProductsName = "Smartphone", ProductsDescreption = "Latest model smartphone", Price = 800, CategoryId = 100, ImgUrl="2.jpg" },
                new Product { ProductsId = 3, ProductsName = "Headphones", ProductsDescreption = "Noise cancelling headphones", Price = 100, CategoryId = 105, ImgUrl="3.jpg" }
            };

            var repo = CreateRepo(products);

            // Act
            var (items, totalCount) = await repo.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: null,
                position: 1,
                skip: 10
            );

            // Assert
            Assert.NotNull(items);
            Assert.Equal(3, items.Count);
            Assert.Equal(3, totalCount);
        }
        [Fact]
        public async Task GetProducts_FilterByDescriptionPriceAndCategory_ReturnsOnlyMatching()
        {
            // Arrange
            int cat = 100;

            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductsName="Laptop", ProductsDescreption="High performance laptop", Price=1200m, CategoryId=cat, ImgUrl="1.jpg" },
                new Product { ProductsId = 2, ProductsName="Smartphone", ProductsDescreption="Latest model smartphone", Price=800m, CategoryId=cat, ImgUrl="2.jpg" },
                new Product { ProductsId = 3, ProductsName="Headphones", ProductsDescreption="Noise cancelling headphones", Price=100m, CategoryId=cat, ImgUrl="3.jpg" }
            };

            var repo = CreateRepo(products);

            // Act
            var (items, totalCount) = await repo.GetProducts(
                description: "smart",
                minPrice: 50,
                maxPrice: 1000,
                categoriesId: new int[] { cat },
                position: 1,
                skip: 8
            );

            // Assert
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Equal(1, totalCount);
            Assert.Equal("Smartphone", items.First().ProductsName);
        }

        [Fact]
        public async Task GetProducts_WhenNoMatch_ReturnsEmpty()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductsName="Laptop", ProductsDescreption="High performance", Price=1200, CategoryId=100, ImgUrl="1.jpg" }
            };

            var repo = CreateRepo(products);

            // Act
            var (items, totalCount) = await repo.GetProducts(
                description: "NonExisting",
                minPrice: 1000,
                maxPrice: 2000,
                categoriesId: new int[] { 105 },
                position: 1,
                skip: 8
            );

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
            Assert.Equal(0, totalCount);
        }

        [Fact]
        public async Task GetProducts_EmptyCategoriesIdArray_DoesNotFilterByCategory()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductsName="Doll", ProductsDescreption="Kids toy", Price=50, CategoryId=1, ImgUrl="1.jpg" },
                new Product { ProductsId = 2, ProductsName="Laptop", ProductsDescreption="Work device", Price=1200, CategoryId=2, ImgUrl="2.jpg" }
            };

            var repo = CreateRepo(products);

            // Act
            var (items, totalCount) = await repo.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: new int[] { },   
                position: 1,
                skip: 8
            );

            // Assert
            Assert.Equal(2, items.Count);
            Assert.Equal(2, totalCount);
        }

        [Fact]
        public async Task GetProducts_Pagination_ReturnsCorrectPage()
        {
            // Arrange
            int cat = 100;

            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductsName="A", ProductsDescreption="d", Price=10m, CategoryId=cat, ImgUrl="1.jpg" },
                new Product { ProductsId = 2, ProductsName="B", ProductsDescreption="d", Price=20, CategoryId=cat, ImgUrl="2.jpg" },
                new Product { ProductsId = 3, ProductsName="C", ProductsDescreption="d", Price=30m, CategoryId=cat, ImgUrl="3.jpg" }
            };

            var repo = CreateRepo(products);

            // Act
            var (items, totalCount) = await repo.GetProducts(
                description: null,
                minPrice: null,
                maxPrice: null,
                categoriesId: new int[] { cat },
                position: 2,
                skip: 2
            );

            // Assert
            Assert.Equal(3, totalCount);
            Assert.Single(items);
        }
    }
}

