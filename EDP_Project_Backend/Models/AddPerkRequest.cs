using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddPerkRequest
    {
        [Range(0, 100)]
        public int PercentageDiscount {  get; set; }

        [Range(0, float.MaxValue)]
        public float FixedDiscount { get; set; }

        [Range(1, int.MaxValue)]
        public int MinGroupSize { get; set; }

        [Required, Range(0, float.MaxValue)]
        public float MinSpend { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int VoucherQuantity { get; set; }

        [Required]
        public int TierId { get; set; }
    }
}
