
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDP_Project_Backend.Models
{
    public class Ticket
    {
        private User? user;

        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public User? User { get => user; set => user = value; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }

        public string IssueType { get; set; }

        [MaxLength(300)]
        public string Complaint { get; set; }

        public string Contact { get; set; }

	}
       
}
