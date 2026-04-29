using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Collections.Generic;
using Repositeries;
using Service;
using DTOs;

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
        public async Task<ActionResult<OrderDTO>> Get(int id)
        {
            var order = await _orderService.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> AddOrder([FromBody] OrderCreateDTO orderDto)
        {
            OrderDTO _orderdto = await _orderService.AddOrder(orderDto);
            if (_orderdto == null)
            {
                return BadRequest("Payment Error: Order sum mismatch.");
            }
            return CreatedAtAction(nameof(Get), new { id = _orderdto.OrderId }, _orderdto);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
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

    }
}