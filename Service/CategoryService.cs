using AutoMapper;
using DTOs;
using Entities;
using Repositeries;




namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetCategory()
        {
          List<Category> cayegoryList = await _categoryRepository.GetCategory();


            List<CategoryDTO> categoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(cayegoryList);
            return categoryDtos;
        }


   
    }
}