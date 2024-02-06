using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class UpdateProfileRequest
    {
        [Required, MinLength(3), MaxLength(50)]
        // Regular expression to enforce name format
        [RegularExpression(@"^[a-zA-Z '-,.]+$", ErrorMessage = "Only allow letters, spaces and characters: ' - , .")]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(50)]
        public string UserEmail { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(8)]
        public string UserHp { get; set; } = string.Empty;
    }
}
