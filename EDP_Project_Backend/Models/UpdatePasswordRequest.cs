using System.ComponentModel.DataAnnotations;

namespace EDP_Project_Backend.Models
{
	public class UpdatePasswordRequest
	{
		[Required, MinLength(8), MaxLength(50)]
		[RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9]).{8,}$", ErrorMessage = "At least 1 letter and 1 number")]
		public string UserPassword { get; set; } = string.Empty;
	}
}
