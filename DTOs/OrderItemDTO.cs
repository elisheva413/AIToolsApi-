using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderItemDTO
    {
        public int OrderItemId { get; init; }
        public int ProductsId { get; init; }
        public string ProductName { get; init; }
        public string ProductImage { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }

        public OrderItemDTO() { }
    }
}

