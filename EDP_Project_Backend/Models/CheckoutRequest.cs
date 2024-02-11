namespace EDP_Project_Backend.Models
{
	public class CheckoutRequest
	{
		public List<StripeItems> SelectedCartItems { get; set; }
		public int? SelectedVoucherId { get; set; }
	}
}
