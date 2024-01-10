using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class ListingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Gprice { get; set; }

        public int Uprice { get; set; }

        public int Nprice { get; set; }

        public int Capacity { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
