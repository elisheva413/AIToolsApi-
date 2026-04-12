using Entities;
using Microsoft.EntityFrameworkCore;
using Repositeries;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Threading.Tasks;



namespace Repositeries
{
    public class ProductRepository : IProductRepository
    {
        Store_215962135Context _store_215962135Context;
        public ProductRepository(Store_215962135Context store_215962135Context)
        {
            _store_215962135Context = store_215962135Context;
        }

        public async Task<(IEnumerable<Product> products, int total)> GetProducts(int[]? categoryId, string? q, decimal? minPrice, decimal? maxPrice, string? color, string? material, bool? inStock, bool? isActive, string? sort, int? skip, int? position)
        {
            int pageSize = (skip.HasValue && skip.Value > 0) ? skip.Value : 12;
            int page = (position.HasValue && position.Value > 0) ? position.Value : 1;

            var query = _store_215962135Context.Products.AsQueryable();

            //bool activeFilter = isActive ?? true;
            //query = query.Where(p => p.IsActive == activeFilter);
           
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            if (categoryId != null && categoryId.Length > 0)
                query = query.Where(p => categoryId.Contains(p.CategoryId));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
            
            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorList = color.Split(',').Select(c => c.Trim()).ToList();
                query = query.Where(p => p.Color != null && colorList.Contains(p.Color));
            }

            if (!string.IsNullOrWhiteSpace(material))
            {
                var materialList = material.Split(',').Select(m => m.Trim()).ToList();
                query = query.Where(p => p.Material != null && materialList.Contains(p.Material));
            }

            if (inStock == true)
                query = query.Where(p => p.Quantity > 0);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.ProductsName.Contains(q) ||
                    p.ProductsDescreption.Contains(q) ||
                    (p.Color != null && p.Color.Contains(q)) ||
                    (p.Material != null && p.Material.Contains(q))
                );
            }

            if (string.Equals(sort, "desc", StringComparison.OrdinalIgnoreCase))
                query = query.OrderByDescending(p => p.Price);
            else if (string.Equals(sort, "asc", StringComparison.OrdinalIgnoreCase))
                query = query.OrderBy(p => p.Price);
            else
                query = query.OrderBy(p => p.ProductsId);  

            Console.WriteLine(query.ToQueryString());

            int total = await query.CountAsync();

            List<Product> products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync();

            return (products, total);
        }


        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _store_215962135Context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductsId == productId);
        }

        public async Task<Product> AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductsName))
                throw new ArgumentException("שם מוצר הוא שדה חובה");

            product.ProductsDescreption = product.ProductsDescreption ?? "";
            product.ImgUrl = product.ImgUrl ?? "";
            product.ImgUrl2 = product.ImgUrl2 ?? "";
            product.IsActive = true;

            await _store_215962135Context.Products.AddAsync(product);
            await _store_215962135Context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProduct(int id, Product product)
        {
            product.ProductsId = id;

            _store_215962135Context.Products.Update(product);
            await _store_215962135Context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _store_215962135Context.Products
                .Include(p => p.OrdersItems) 
                .FirstOrDefaultAsync(p => p.ProductsId == id);

            if (product != null)
            {
                if (product.OrdersItems.Any())
                {
                    product.IsActive = false;
                    _store_215962135Context.Products.Update(product);
                }
                else
                {
                    _store_215962135Context.Products.Remove(product);
                }
                await _store_215962135Context.SaveChangesAsync();
            }
        }
      
    }
}
