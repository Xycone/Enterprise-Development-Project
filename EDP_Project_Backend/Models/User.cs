using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class User
    {

        public int Id { get; set; }

        public Boolean IsAdmin {  get; set; }

        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(100), JsonIgnore]
        public string UserPassword { get; set; } = string.Empty;

        public string UserPicture { get; set; } = string.Empty;

        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [MaxLength(15)]
        public string UserHp { get; set; } = string.Empty;

        // Stores the amt of extra spendings not used to level up a tier
        [Range(0, int.MaxValue)]
        public float TotalSpent { get; set; }

        // Stores the amt of extra bookings not used to level up a tier
        [Range(0, int.MaxValue)]
        public int TotalBookings { get; set; }

        // Foreign key
        // User belongs to a tier
        public int? TierId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public Tier? Tier { get; set; }

        // Navigation property to represent the one-to-many relationship
        // Represent the User's relationship to the voucher class
        [JsonIgnore]
        public List<Voucher>? Vouchers { get; set; }

		// Navigation property to represent the one-to-many relationship
		// Represent the User's relationship to the tickets class
		[JsonIgnore]
		public List<Ticket>? Tickets { get; set; }

		// Navigation property to represent the one-to-many relationship
		// Represent the User's relationship to the reviews class
		[JsonIgnore]
		public List<Review>? Reviews { get; set; }

		[JsonIgnore]
		public List<CartItem>? CartItems { get; set; }

		[Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

    }
}
