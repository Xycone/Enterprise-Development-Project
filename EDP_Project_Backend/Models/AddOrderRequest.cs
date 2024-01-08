
using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddOrderRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0.")]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "OrderTotal is required.")]
        [Range(1, float.MaxValue, ErrorMessage = "OrderTotal must be greater than 0.")]
        public float OrderTotal { get; set; }
    }
}