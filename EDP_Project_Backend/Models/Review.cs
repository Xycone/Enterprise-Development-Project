using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EDP_Project_Backend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int starRating { get; set; }
        public string Desc { get; set; }
        public string UserName { get; set; }       

        // Foreign key
        // Review belongs to a user
		public int UserId { get; set; }
		public User? User { get; set; }
        //FK => Review belongs to a Activity
        public Activity? Activity { get; set; }
        public int ActivityId { get; set; }
    }
}

