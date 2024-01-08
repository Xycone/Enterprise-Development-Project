namespace EDP_Project_Backend.Models
{
    public class PerkDTO
    {
        public int Id { get; set; }

        public int PercentageDiscount { get; set; }

        public float FixedDiscount { get; set; }

        public int MinGroupSize { get; set; }

        public float MinSpend { get; set; }

        public int VoucherQuantity { get; set; }

        public string TierName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
