using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderDTO
    {
        public int OrderId { get; init; }
        public DateOnly OrderDate { get; init; }
        public decimal OrderSum { get; init; }
        public int UserId { get; init; }
        public string OrderStatus { get; init; }
        public List<OrderItemDTO> OrdersItems { get; init; }

        public OrderDTO() { }
    }
}
    

