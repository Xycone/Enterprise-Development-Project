using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class UpdateOrderRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        [Required]
        public float OrderTotal { get; set; }
    }

}
