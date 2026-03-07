using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Repositeries;
using Service;
using System.Collections.Generic;




namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
       

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
           
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategory()
        {
            List<CategoryDTO> categories = await _categoryService.GetCategory();
            if (categories == null || categories.Count == 0)
                return NoContent();
            return Ok(categories);
        }

     


    }
}