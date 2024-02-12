
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

namespace EDP_Project_Backend.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public OrderController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAllOrders([FromQuery] int? userId)
        {
            IQueryable<Order> result = _context.Orders;

            // Filter orders by userId if the userId parameter is provided
            if (userId.HasValue)
            {
                result = result.Where(o => o.UserId == userId.Value);
            }


            var list = result.ToList();
            IEnumerable<OrderDTO> data = list.Select(order => _mapper.Map<OrderDTO>(order));

            return Ok(data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
        public IActionResult GetOrderById(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            var orderDTO = _mapper.Map<OrderDTO>(order);

            return Ok(orderDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
        public IActionResult AddOrder([FromBody] AddOrderRequest orderRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        );
                    
                return BadRequest(new { Errors = errors });
            }

            // Use orderRequest.UserId to associate the order with the specified user
            int userId = orderRequest.UserId;

            // Example: Create a new Order entity
            var newOrder = new Order
            {
                UserId = userId,
                ActivityName = orderRequest.ActivityName,
                Quantity = orderRequest.Quantity,
                TotalPrice = orderRequest.TotalPrice,
                OrderDate = orderRequest.OrderDate
            };

            // Save the new order to the database
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            // Map the created order back to a DTO for response
            var createdOrderDTO = _mapper.Map<OrderDTO>(newOrder);

            return Ok(createdOrderDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
