using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EDP_Project_Backend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ActivityName { get; set; }

        public int Quantity { get; set; }


        public float TotalPrice { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
    }
}