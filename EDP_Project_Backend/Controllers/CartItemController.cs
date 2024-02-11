using AutoMapper;
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EDP_Project_Backend.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CartItemController : ControllerBase
	{
		private readonly MyDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<CartItemController> _logger;

		public CartItemController(MyDbContext context, IMapper mapper,
			ILogger<CartItemController> logger)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status200OK)]
		public IActionResult GetCartItem(int id)
		{
			CartItem? ci = _context.CartItems.Find(id);
			if (ci == null)
			{
				return NotFound();
			}
			CartItemDTO data = _mapper.Map<CartItemDTO>(ci);
			return Ok(data);
		}

		[HttpGet("GetCartItems")]
		[ProducesResponseType(typeof(IEnumerable<CartItemDTO>), StatusCodes.Status200OK)]
		public IActionResult GetAll()
		{
			int userid = GetUserId();
			try
			{
				IQueryable<CartItem> result = _context.CartItems.Include(t => t.User);
					result = result.Where(x => x.UserId == userid);
				var list = result.OrderByDescending(x => x.CreatedAt).ToList();
				IEnumerable<CartItemDTO> data = list.Select(t => _mapper.Map<CartItemDTO>(t));
				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when get all CartItems");
				return StatusCode(500);
			}
		}

		[HttpPost("AddCartItems"), Authorize]
		[ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status200OK)]
		public IActionResult AddCartItem(AddCartItemRequest CartItem)
		{
			try
			{
				int userId = GetUserId();
				var now = DateTime.Now;
				var myCartItem = new CartItem()
				{
					Name = CartItem.Name.Trim(),
					Quantity = CartItem.Quantity,
					Price = CartItem.Price,
					Total_Price = CartItem.Quantity * CartItem.Price,
					CreatedAt = now,
					UpdatedAt = now,
					UserId = userId,
					ActivityId = CartItem.ActivityId,
				};

				_context.CartItems.Add(myCartItem);
				_context.SaveChanges();

				CartItem? newCartItem = _context.CartItems.Include(t => t.User)
					.FirstOrDefault(t => t.Id == myCartItem.Id);
				CartItemDTO CartItemDTO = _mapper.Map<CartItemDTO>(newCartItem);
				return Ok(CartItemDTO);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when add CartItem");
				return StatusCode(500);
			}
		}

		[HttpPut("{id}"), Authorize]
		public IActionResult UpdateCartItem(int id, UpdateCartItemRequest CartItem)
		{
			try
			{
				var myCartItem = _context.CartItems.Find(id);
				if (myCartItem == null)
				{
					return NotFound();
				}

				int userId = GetUserId();
				if (myCartItem.UserId != userId)
				{
					return Forbid();
				}

				if (CartItem.Name != null)
				{
					myCartItem.Name = CartItem.Name.Trim();
				}
				if (CartItem.Quantity != myCartItem.Quantity)
				{
					myCartItem.Quantity = CartItem.Quantity;
					myCartItem.Total_Price = myCartItem.Quantity * myCartItem.Price;
				}
				if (CartItem.Price != myCartItem.Price)
				{
					myCartItem.Price = CartItem.Price;
					myCartItem.Total_Price = myCartItem.Quantity * myCartItem.Price;
				}
				myCartItem.UpdatedAt = DateTime.Now;

				_context.SaveChanges();
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when update CartItem");
				return StatusCode(500);
			}
		}

		[HttpDelete("{id}"), Authorize]
		public IActionResult DeleteCartItem(int id)
		{
			try
			{
				var myCartItem = _context.CartItems.Find(id);
				if (myCartItem == null)
				{
					return NotFound();
				}

				int userId = GetUserId();
				if (myCartItem.UserId != userId)
				{
					return Forbid();
				}

				_context.CartItems.Remove(myCartItem);
				_context.SaveChanges();
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when delete CartItem");
				return StatusCode(500);
			}
		}

		[HttpDelete, Authorize]
		public IActionResult DeleteAllCartItems()
		{
			try
			{
				int userId = GetUserId();

				var userCartItems = _context.CartItems.Where(item => item.UserId == userId).ToList();

				if (userCartItems == null || userCartItems.Count == 0)
				{
					return NotFound("No cart items found for the user.");
				}

				_context.CartItems.RemoveRange(userCartItems);
				_context.SaveChanges();

				return Ok("All cart items deleted successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when deleting all CartItems");
				return StatusCode(500, "Internal server error");
			}
		}


		private int GetUserId()
		{
			return Convert.ToInt32(User.Claims
				.Where(c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value).SingleOrDefault());
		}
	}
}
