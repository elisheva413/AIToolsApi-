using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Entities;
using System.Collections.Generic;
using Repositeries;
using Service;
using DTOs;
using static Service.IProductService;
using AutoMapper;
using WebApiShop.Security;




namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper; 
        private readonly IWebHostEnvironment _env;


        public ProductsController(IProductService productsService, IMapper mapper, IWebHostEnvironment env)
        {
            _productService = productsService;
            _mapper = mapper;
            _env = env;
        }
        [HttpGet]
        [AllowAnonymous]
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
            var result = await _productService.GetProducts(
                categoryId, q, minPrice, maxPrice,
                color, material, inStock, isActive,
                sort, skip, position);

            if (result.Items == null || result.Items.Count == 0)
                return NoContent();

            return Ok(result);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var productDto = await _productService.GetProductByIdAsync(id);
            if (productDto == null)
                return NotFound();
            return Ok(productDto);

        }
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var newProduct = await _productService.AddProduct(product);

            var resultDto = _mapper.Map<ProductDTO>(newProduct);
            return CreatedAtAction(nameof(GetById), new { id = resultDto.ProductsId }, resultDto);
        }

        [HttpPut("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDTO productDto)
        {
            var productToUpdate = _mapper.Map<Product>(productDto);
            var updatedProduct = await _productService.UpdateProduct(id, productToUpdate);

            if (updatedProduct == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound(new { message = "המוצר לא נמצא או שלא ניתן למחוק אותו" });
            }
        }

        [HttpPost("upload-image")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("לא נבחר קובץ");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "products");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{uniqueFileName}";
            return Ok(new { imageUrl });
        }
    }
}