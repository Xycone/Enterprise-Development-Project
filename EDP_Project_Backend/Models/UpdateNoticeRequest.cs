using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class UpdateNoticeRequest
    {
        [Required, MinLength(3), MaxLength(15)]
        public string Name { get; set; }
        [Required, MinLength(3)]
        public string Description { get; set; }
    }
}
