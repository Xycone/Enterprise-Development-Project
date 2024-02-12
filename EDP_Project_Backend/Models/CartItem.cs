using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDP_Project_Backend.Models
{
	public class CartItem
	{
		public int Id { get; set; }
		[MaxLength(128)]
		public string Name { get; set; } = string.Empty;
		[Range(1,int.MaxValue)]
		public int Quantity { get; set; }
		[Range(0.01,float.MaxValue)]
		public float Price { get; set; }
		[Range(0.01, float.MaxValue)]
		public float Total_Price { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; }
		public int UserId { get; set; }
		public User? User { get; set; }
		public int ActivityId { get; set; }
		public Activity? Activity { get; set; }
	}
}
