using AutoMapper;
using DTOs;
using Entities;
using Repositeries;



namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
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
            Order newOrder = await _orderRepository.AddOrder(order);
            OrderDTO orderDto = _mapper.Map<Order, OrderDTO>(order);
            return orderDto;

           
        }
            
    }
}