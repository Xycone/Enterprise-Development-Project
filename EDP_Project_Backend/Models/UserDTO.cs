namespace EDP_Project_Backend.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public Boolean IsAdmin { get; set; }
    }
}
