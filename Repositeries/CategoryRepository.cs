using Entities;
using Microsoft.EntityFrameworkCore;
using Repositeries;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Threading.Tasks;



namespace Repositeries
{
    public class CategoryRepository : ICategoryRepository
    {
        Store_215962135Context _store_215962135Context;
        public CategoryRepository(Store_215962135Context store_215962135Context)
        {
            _store_215962135Context = store_215962135Context;
        }


        public async Task<List<Category>> GetCategory()
        {
            return await _store_215962135Context.Categories.ToListAsync();
        }




    }
}
