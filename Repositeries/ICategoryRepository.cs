using Entities;

namespace Repositeries;
public interface ICategoryRepository
{
    Task<List<Category>> GetCategory();
}