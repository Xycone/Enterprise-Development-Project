using System;
using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
    public class AddOrderRequest
    {

        public int UserId { get; set; }


        public string ActivityName { get; set; }


        public int Quantity { get; set; }

        public float TotalPrice { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }
    }
}