namespace EDP_Project_Backend.Models
{
    public class UserProfileDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string UserHp { get; set; } = string.Empty;

        public float TotalSpent { get; set; }

        public int TotalBookings { get; set; }

        public string? TierName { get; set; } = string.Empty;
    }
}
