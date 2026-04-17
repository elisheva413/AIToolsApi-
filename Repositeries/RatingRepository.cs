using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repositeries
{
    public class RatingRepository : IRatingRepository
    {
        private readonly Store_215962135Context _context;

        public RatingRepository(Store_215962135Context context)
        {
            _context = context;
        }

        public async Task<Rating> AddRating(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
            return rating;
        }
    }
}



