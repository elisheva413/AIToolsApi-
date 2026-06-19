using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Entities;
using System.Collections.Generic;
using Repositeries;
using Service;
using DTOs;
using System.Security.Claims;
using WebApiShop.Security;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("all")]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        //{
        //    var orders = await _orderService.GetAllOrders();
        //    return Ok(orders);
        //}

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> Get(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            bool isAdmin = User.IsInRole("Admin");
            int? currentUserId = GetCurrentUserId();
            if (!isAdmin && (!currentUserId.HasValue || order.UserId != currentUserId.Value))
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> AddOrder([FromBody] OrderCreateDTO orderDto)
        {
            bool isAdmin = User.IsInRole("Admin");
            int? currentUserId = GetCurrentUserId();
            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                {
                    return Unauthorized("User id claim is missing.");
                }

                if (orderDto.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            OrderDTO _orderdto = await _orderService.AddOrder(orderDto);
            if (_orderdto == null)
            {
                return BadRequest("Payment Error: Order sum mismatch.");
            }
            return CreatedAtAction(nameof(Get), new { id = _orderdto.OrderId }, _orderdto);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUserId(int userId)
        {
            bool isAdmin = User.IsInRole("Admin");
            int? currentUserId = GetCurrentUserId();
            if (!isAdmin && (!currentUserId.HasValue || userId != currentUserId.Value))
            {
                return Forbid();
            }

            var orders = await _orderService.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult<OrderDTO>> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            var validStatuses = new[] { "Paid", "Shipped", "Delivered" };
            if (!validStatuses.Contains(newStatus))
            {
                return BadRequest("Invalid status. Allowed values are: Paid, Shipped, Delivered.");
            }

            var updatedOrder = await _orderService.UpdateOrderStatus(orderId, newStatus);

            if (updatedOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedOrder);
        }

        private int? GetCurrentUserId()
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

    }
}