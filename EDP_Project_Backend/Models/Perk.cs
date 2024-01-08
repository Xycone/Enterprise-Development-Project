using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class Perk
    {
        // Primary key
        public int Id { get; set; }

        // Stores discount that offer a percentage off the price
        [Range(1, 100)]
        public int PercentageDiscount { get; set; }

        // Stores discount that offers a fixed value off
        [Range(0.01, float.MaxValue)]
        public float FixedDiscount { get; set; }

        // Stores min group size
        [Required, Range(2, int.MaxValue)]
        public int MinGroupSize { get; set; }

        // Stores min spend for voucher to be applied
        [Required, Range(0, float.MaxValue)]
        public float MinSpend { get; set;}

        // Number of vouchers the perk for the tier would give to the users belonging to said tier every month
        [Required, Range(1, int.MaxValue)]
        public int VoucherQuantity { get; set; }

        // Foreign key
        // Each tier has its perks
        public int TierId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public Tier? Tier { get; set; }

        // Navigation property to represent the one-to-many relationship
        // Represent the Perk's relationship to the voucher class
        [JsonIgnore]
        public List<Voucher>? Vouchers { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
    }
}
