using Entities;
using System.Threading.Tasks;

namespace Service
{
    public interface IRatingService
    {
        Task<Rating> AddRating(Rating rating);
    }
}