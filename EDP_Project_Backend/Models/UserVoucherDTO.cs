namespace EDP_Project_Backend.Models
{
	public class UserVoucherDTO
	{
		public int Id { get; set; }

		public int PercentageDiscount { get; set; }

		public float FixedDiscount { get; set; }

		public int MinGroupSize { get; set; }

		public float MinSpend { get; set; }

		public int VoucherQuantity { get; set; }

		public DateTime DiscountExpiry { get; set; }
	}
}
