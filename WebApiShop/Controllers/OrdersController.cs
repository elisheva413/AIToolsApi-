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

        [HttpGet]

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> Get(int id)
        {
            var order = await _orderService.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> AddOrder([FromBody] Order order)
        {
            OrderDTO _orderdto = await _orderService.AddOrder(order);

            if (_orderdto == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new { Id = order.OrderId }, order);
        }    

    }
}