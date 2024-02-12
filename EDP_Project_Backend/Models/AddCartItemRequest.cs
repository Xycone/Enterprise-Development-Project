using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
	public class AddCartItemRequest
	{
		[Required, MinLength(3), MaxLength(128)]
		public string Name { get; set; } = string.Empty;
		[Required, Range(1, int.MaxValue)]
		public int Quantity { get; set; }
		[Required, Range(0, float.MaxValue)]
		public float Price { get; set; }
		[Required]
		public int ActivityId { get; set; }
	}
}
