using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class Tier
    {
        // Primary Key
        public int Id { get; set; }

        // Name of the tier (e.g. classic, bronze, silver, gold, etc.)
        [Required, MinLength(3), MaxLength(15)]
        public string TierName { get; set; } = string.Empty;

        // Number of events the user needs to book to level up from this tier to the next
        [Required, Range(1, int.MaxValue)]
        public int TierBookings { get; set; }

        // Amount the user needs to spend to level up from this tier to the next
        [Required, Range(1, int.MaxValue)]
        public float TierSpendings { get; set; }

        // Tier position relative to other
        public int TierPosition { get; set; }

        // Navigation property to represent the one-to-many relationship
        // Represent the Tier's relationship to the user class
        [JsonIgnore]
        public List<User>? Users { get; set; }

        // Navigation property to represent the one-to-many relationship
        // Represent the Tier's relationship to the perk class
        [JsonIgnore]
        public List<Perk>? Perks { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
    }
}
