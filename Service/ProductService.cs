using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Repositeries;


namespace Service
{
    public class ProductService : IProductService
    {
        private const string ProductsCacheKey = "products:all";
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _cache;
        private readonly int _productsTtlSeconds;
        IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper, IDistributedCache cache, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _cache = cache;
            _productsTtlSeconds = configuration.GetValue<int?>("CacheSettings:ProductsTtlSeconds") ?? 60;
        }

        public async Task<FinalProducts> GetProducts(int[]? categoryId, string? q, decimal? minPrice, decimal? maxPrice, string? color, string? material, bool? inStock, bool? isActive, string? sort, int? skip, int? position)
        {
            bool isUnfilteredRequest = (categoryId == null || categoryId.Length == 0) &&
                                      string.IsNullOrWhiteSpace(q) &&
                                      minPrice == null &&
                                      maxPrice == null &&
                                      string.IsNullOrWhiteSpace(color) &&
                                      string.IsNullOrWhiteSpace(material) &&
                                      inStock == null &&
                                      isActive == null &&
                                      string.IsNullOrWhiteSpace(sort) &&
                                      skip == null &&
                                      position == null;

            if (isUnfilteredRequest)
            {
                var cached = await _cache.GetStringAsync(ProductsCacheKey);
                if (!string.IsNullOrWhiteSpace(cached))
                {
                    var cachedResult = JsonSerializer.Deserialize<FinalProducts>(cached);
                    if (cachedResult != null)
                    {
                        return cachedResult;
                    }
                }
            }

            var (products, total) = await _productRepository.GetProducts(
                categoryId, q, minPrice, maxPrice, color, material,
                inStock, isActive, sort, skip, position);

            var itemsDto = _mapper.Map<List<ProductDTO>>(products);

            int pageSize = (skip.HasValue && skip.Value > 0) ? skip.Value : 12;
            int page = (position.HasValue && position.Value > 0) ? position.Value : 1;

            bool hasNext = (total - (page * pageSize)) > 0;
            bool hasPrev = page > 1;

            var result = new FinalProducts(itemsDto, total, hasNext, hasPrev);

            if (isUnfilteredRequest)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_productsTtlSeconds)
                };

                var payload = JsonSerializer.Serialize(result);
                await _cache.SetStringAsync(ProductsCacheKey, payload, options);
            }

            return result;
        }
        public async Task<ProductDTO?> GetProductByIdAsync(int productsId)
        {
            var product = await _productRepository.GetProductByIdAsync(productsId);
            if (product == null)
                return null;

            var productDto = _mapper.Map<ProductDTO>(product);
            return productDto;
        }

        public async Task<Product> AddProduct(Product product)
        {
            var created = await _productRepository.AddProduct(product);
            await InvalidateProductsCache();
            return created;
        }

        public async Task<Product> UpdateProduct(int id, Product product)
        {
            var updated = await _productRepository.UpdateProduct(id, product);
            await InvalidateProductsCache();
            return updated;
        }

        public async Task DeleteProduct(int id)
        {
            await _productRepository.DeleteProduct(id);
            await InvalidateProductsCache();
        }

        private async Task InvalidateProductsCache()
        {
            await _cache.RemoveAsync(ProductsCacheKey);
        }
        //public async Task<bool> SoftDeleteProduct(int id)
        //{
        //    return await _productRepository.SoftDeleteProduct(id);
        //}

    }
}