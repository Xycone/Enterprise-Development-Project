namespace EDP_Project_Backend.Models
{
    public class TierDTO
    {
        public int Id { get; set; }

        public string TierName { get; set; } = string.Empty;

        public int TierBookings { get; set; }

        public float TierSpendings { get; set; }

        public int TierPosition { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set;}
    }
}
