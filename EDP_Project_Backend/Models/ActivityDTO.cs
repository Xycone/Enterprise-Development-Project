namespace EDP_Project_Backend.Models
{
    public class ActivityDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int AvailSpots { get; set; }
        public int ActivityListingId { get; set; }
    }
}
