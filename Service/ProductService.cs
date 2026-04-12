using AutoMapper;
using DTOs;
using Entities;
using Repositeries;


namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<FinalProducts> GetProducts(int[]? categoryId, string? q, decimal? minPrice, decimal? maxPrice, string? color, string? material, bool? inStock, bool? isActive, string? sort, int? skip, int? position)
        {
            var (products, total) = await _productRepository.GetProducts(
                categoryId, q, minPrice, maxPrice, color, material,
                inStock, isActive, sort, skip, position);

            var itemsDto = _mapper.Map<List<ProductDTO>>(products);

            int pageSize = (skip.HasValue && skip.Value > 0) ? skip.Value : 12;
            int page = (position.HasValue && position.Value > 0) ? position.Value : 1;

            bool hasNext = (total - (page * pageSize)) > 0;
            bool hasPrev = page > 1;

            return new FinalProducts(itemsDto, total, hasNext, hasPrev);
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
            return await _productRepository.AddProduct(product);
        }

        public async Task<Product> UpdateProduct(int id, Product product)
        {
            return await _productRepository.UpdateProduct(id, product);
        }

        public async Task DeleteProduct(int id)
        {
            await _productRepository.DeleteProduct(id);
        }
        //public async Task<bool> SoftDeleteProduct(int id)
        //{
        //    return await _productRepository.SoftDeleteProduct(id);
        //}

    }
}