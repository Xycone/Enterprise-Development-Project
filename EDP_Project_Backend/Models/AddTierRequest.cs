using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddTierRequest
    {
        [Required, MinLength(3), MaxLength(15)]
        public string TierName { get; set; } = string.Empty;

        [Required, Range(1, int.MaxValue)]
        public int TierBookings { get; set; }

        [Required, Range(1, int.MaxValue)]
        public float TierSpendings { get; set; }

    }
}
