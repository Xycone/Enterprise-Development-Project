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
        public int PercentageDiscount { get; set; }

        // Stores discount that offers a fixed value off
        public float FixedDiscount { get; set; }

        // Stores min group size if this voucher is a group voucher
        // Left blank if this voucher isnt a group voucher
        public int MinGroupSize { get; set; }

        // Stores min spend for voucher to be applied
        [Required, Range(0, float.MaxValue)]
        public float MinSpend { get; set;}

        // Number of vouchers the perk for the tier would give to the users belonging to said tier every month
        [Required, Range(0, int.MaxValue)]
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
