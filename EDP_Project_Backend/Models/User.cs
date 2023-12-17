using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public Boolean IsAdmin {  get; set; }

        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(100), JsonIgnore]
        public string UserPassword { get; set; } = string.Empty;

        public string UserPicture { get; set; } = string.Empty;

        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [MaxLength(15)]
        public string UserHp { get; set; } = string.Empty;

        // Stores the amt of extra spendings not used to level up a tier
        [Range(1, int.MaxValue)]
        public float TotalSpent { get; set; }

        // Stores the amt of extra bookings not used to level up a tier
        [Range(1, int.MaxValue)]
        public int TotalBookings { get; set; }

        // Foreign key
        public int TierId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public Tier? Tier { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
    }
}
