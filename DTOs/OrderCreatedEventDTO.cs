using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderCreatedEventDTO
    (
        int OrderId,
        int UserId,
        DateOnly OrderDate,
        decimal OrderSum,
        string OrderStatus,
        List<OrderItemDTO> OrderItems
    );
}
