using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class ActivityListing
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Category {  get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0, 500)]
        public int Gprice { get; set; }

        [Required, Range(0, 500)]
        public int Uprice { get; set; }

        [Required, Range(0, 500)]
        public int Nprice { get; set; }

        [Required, Range(0, 500)]
        public int Capacity { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
