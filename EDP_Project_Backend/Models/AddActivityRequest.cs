using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddActivityRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Required, Range(0, 500)]
        public int AvailSpots { get; set; }

        [Required]
        public int ActivityListingId { get; set; }
    }
}
