using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record ProductDTO
    (
            int ProductsId,
            int CategoryId,
            string ProductsName,
            string ProductsDescreption,
            decimal Price,
            string ImgUrl,
            string ImgUrl2,
			string Color,
            string Material,
            int Quantity,
            bool IsActive


    );
  
}
