using AutoMapper;
using DTOs;
using Entities;
using Repositeries;



namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IProductService productService, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<OrderDTO?> GetOrderById(int id)
        {
            Order? order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return null;

            return _mapper.Map<OrderDTO>(order);
        }


        //public async Task<OrderDTO> AddOrder(Order order)
        //{
        //    Order newOrder = await _orderRepository.AddOrder(order);
        //    var fullOrder = await _orderRepository.GetOrderById(newOrder.OrderId);
        //    return _mapper.Map<Order, OrderDTO>(fullOrder);
        //}
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
                throw new Exception("Payment error");
            }

            Order newOrder = await _orderRepository.AddOrder(order);
            var fullOrder = await _orderRepository.GetOrderById(newOrder.OrderId);
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