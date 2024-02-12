namespace EDP_Project_Backend.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ActivityName { get; set; }

        public int Quantity { get; set; }


        public float TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }
    }
}