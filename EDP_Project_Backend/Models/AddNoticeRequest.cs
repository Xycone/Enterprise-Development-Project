using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddNoticeRequest
    {
        [Required] 
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Name { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; }
       }

}
