using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderCreateDTO
     (
         [Required]
        DateOnly OrderDate,

         [Required]
        decimal OrderSum,

         [Required]
        int UserId,

         List<OrderItemCreteDTO> OrdersItems
     );
}
