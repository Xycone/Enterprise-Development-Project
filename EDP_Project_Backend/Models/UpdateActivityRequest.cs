using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class UpdateActivityRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Range(0, 500)]
        public int AvailSpots { get; set; }

    }
}
