using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EDP_Project_Backend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int activityId { get; set; } // temp until i get activity stuff
        public DateTime Date { get; set; }
        public int starRating { get; set; }
        public string Desc { get; set; }
        public string UserName { get; set; }       

        // Foreign key
        // Review belongs to a user
		public int UserId { get; set; }
		// Navigation property to represent the one-to-many relationship
		public User? User { get; set; }
	}
}

