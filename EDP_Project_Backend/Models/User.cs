using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public Boolean IsAdmin {  get; set; }

        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string UserPassword { get; set; } = string.Empty;

        public string UserPicture { get; set; } = string.Empty;

        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [MaxLength(15)]
        public string UserHp { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public float TotalSpent { get; set; }

        [Range(1, int.MaxValue)]
        public int TotalBookings { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
    }
}
