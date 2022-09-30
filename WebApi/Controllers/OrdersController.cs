using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using WebApi.Dtos;
using AutoMapper;
using System.Linq;
using System.Security.Claims;
using WebApi.Extensions;
using WebApi.Errors;
using System.Collections.Generic;
using Core.Interfaces;

namespace WebApi.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);
            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.basketId, address);

            if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order"));

            return Ok(order);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var orders = await _orderService.GetOrdersForUserAsync(email);

            return Ok(_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(orders));
        }
        [Authorize]
        [HttpGet("{id}")]

        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal(); 
            var order = await _orderService.GetOrderByIdAsync(id, email);
            if (order == null) return NotFound(new ApiResponse(404));
            return _mapper.Map<Order,OrderToReturnDto>(order);
        }
        [Authorize]
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodsAsync());
        }

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllOrders()
        {
            var orders=await _orderService.GetAllOrdersAsync();
            return Ok(_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(orders));
        }
        [HttpPost("accept")]
        public async Task<ActionResult<Order>> AcceptOrder(OrderToReturnDto orderToReturnDto)
        {
            var order=await _orderService.AcceptOrderAsync(orderToReturnDto.Id);
            return Ok(order);
        }
        [HttpPost("reject")]
        public async Task<ActionResult<Order>> RejectOrder(OrderToReturnDto orderToReturnDto)
        {
            var order=await _orderService.RejectOrderAsync(orderToReturnDto.Id);
            return Ok(order);
        }
    }
}