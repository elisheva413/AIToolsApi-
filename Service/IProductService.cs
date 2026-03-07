using Entities;
using DTOs;

namespace Service
{
    public interface IProductService
    {
            Task<FinalProducts> GetProducts(int[]? categoryId, string? q, decimal? minPrice, decimal? maxPrice, string? color, string? material, bool? inStock, bool? isActive, string? sort, int? skip, int? position );
            Task<ProductDTO?> GetProductByIdAsync(int productId);
        }
    }
