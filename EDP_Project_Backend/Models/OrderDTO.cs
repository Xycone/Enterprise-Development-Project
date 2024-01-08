namespace EDP_Project_Backend.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public float OrderTotal { get; set; }
    }

}
