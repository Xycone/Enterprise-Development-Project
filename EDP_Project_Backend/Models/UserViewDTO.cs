namespace EDP_Project_Backend.Models
{
    public class UserViewDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string UserHp { get; set; } = string.Empty;

        public string? TierName { get; set; } = string.Empty;
    }
}
