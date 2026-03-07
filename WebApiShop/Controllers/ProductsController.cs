using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Collections.Generic;
using Repositeries;
using Service;
using DTOs;
using static Service.IProductService;




namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productsService;
       

        public ProductsController(IProductService productsService)
        {
            _productsService = productsService;
           
        }
        [HttpGet]
        public async Task<ActionResult<FinalProducts>> GetProducts(
            [FromQuery] int[]? categoryId,
            [FromQuery] string? q,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? color,
            [FromQuery] string? material,
            [FromQuery] bool? inStock,
            [FromQuery] bool? isActive,
            [FromQuery] string? sort,
            [FromQuery] int? skip,
            [FromQuery] int? position)
        {
            var result = await _productsService.GetProducts(
                categoryId, q, minPrice, maxPrice,
                color, material, inStock, isActive,
                sort, skip, position);

            if (result.Items == null || result.Items.Count == 0)
                return NoContent();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var productDto = await _productsService.GetProductByIdAsync(id);
            if (productDto == null)
                return NotFound();
            return Ok(productDto);
        }

    }
}