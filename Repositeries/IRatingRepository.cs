using Entities;
using System.Threading.Tasks;

namespace Repositeries
{
    public interface IRatingRepository
    {
        Task<Rating> AddRating(Rating rating);
    }
}