using Entities;

namespace Repositeries
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> products, int total)> GetProducts(int[]? categoryId, string? q, decimal? minPrice, decimal? maxPrice, string? color, string? material, bool? inStock, bool? isActive, string? sort, int? skip, int? position);

        Task<Product?> GetProductByIdAsync(int productId);
    }
}