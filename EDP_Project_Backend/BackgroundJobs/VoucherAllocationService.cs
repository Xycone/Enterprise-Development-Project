using EDP_Project_Backend.Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EDP_Project_Backend.BackgroundJobs
{
	public class VoucherAllocationService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public VoucherAllocationService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var voucherAllocator = scope.ServiceProvider.GetRequiredService<allocateVouchers>();
					voucherAllocator.AllocateVouchers();
				}

				await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Runs every day
			}
		}
	}
}
