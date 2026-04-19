using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Logging;
using Repositeries;



namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IProductService productService, IMapper mapper,ILogger<OrderService>logger)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDTO?> GetOrderById(int id)
        {
            Order? order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return null;

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> AddOrder(Order order)
        {
            decimal totalSum = 0;
            foreach (var item in order.OrdersItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductsId);
                if (product != null)
                {
                    totalSum += product.Price * item.Quantity;
                }
            }

            if (order.OrderSum != totalSum)
            {
                _logger.LogWarning($"Security Warning: UserID {order.UserId} attempted to change order sum. Expted {totalSum}, but got {order.OrderSum}");
                return null;
            }

            Order newOrder = await _orderRepository.AddOrder(order);
            var fullOrder = await _orderRepository.GetOrderById(newOrder.OrderId);
            _logger.LogInformation($"Order {newOrder.OrderId} created successfully for UserID {newOrder.UserId} with total sum {totalSum}");
            return _mapper.Map<Order, OrderDTO>(fullOrder);
        }


        public async Task<IEnumerable<OrderDTO>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserId(userId);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO?> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _orderRepository.UpdateOrderStatus(orderId, newStatus);
            if (order == null) return null;
            return _mapper.Map<OrderDTO>(order);
        }

    }
}