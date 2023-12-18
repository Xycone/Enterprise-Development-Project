using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDP_Project_Backend.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required]
        public DateTime DiscountExpiry { get; set; }

        // Foreign key
        // Vouchers belong to a user
        public int UserId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }

        // Foreign key
        // Perks provide the users with monthly vouchers
        public int PerkId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public Perk? Perk { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
    }
}
