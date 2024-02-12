using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required, Range(0, 500)]
        public int AvailSpots { get; set; }

        // Foreign Key
        public int ActivityListingId { get; set; }

        // Navigation property to ActivityListing
        public ActivityListing ActivityListing { get; set; }
        [JsonIgnore]
        public List<CartItem>? CartItems { get; set; }
    }
}
