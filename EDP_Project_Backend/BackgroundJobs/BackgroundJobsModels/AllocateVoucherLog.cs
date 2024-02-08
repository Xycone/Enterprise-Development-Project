using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDP_Project_Backend.BackgroundJobs.BackgroundJobsModels
{
	public class AllocateVoucherLog
	{
		public int Id { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }
	}
}
