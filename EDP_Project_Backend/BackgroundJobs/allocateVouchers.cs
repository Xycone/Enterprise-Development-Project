using EDP_Project_Backend.BackgroundJobs.BackgroundJobsModels;
using EDP_Project_Backend.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Hangfire
{
	public class allocateVouchers
	{
		private readonly MyDbContext _context;

		public allocateVouchers(MyDbContext context)
		{
			_context = context;
		}

		public void AllocateVouchers()
		{
			if (!_context.Perks.Any())
			{
				Console.WriteLine("Data not populated yet");
				return;
			}

			// Retrieve the last time the voucher allocation was performed
			var latestLog = _context.AllocateVoucherLog.OrderByDescending(log => log.CreatedAt).FirstOrDefault();

			if (latestLog != null )
			{
				int currentMonth = DateTime.Now.Month;
				int currentYear = DateTime.Now.Year;

				// Checks if the voucher allocation has already been performed
				if (latestLog.CreatedAt.Month == currentMonth && latestLog.CreatedAt.Year == currentYear)
				{
					Console.WriteLine("Voucher allocation has already been performed this month");
					return;
				}
                else
                {
					try
					{
						var userList = _context.Users.Include(u => u.Tier).ToList();

						foreach (var user in userList)
						{
							// Retrieve the id of the tier the user is in, returns null if user as no tier (admin account)
							var userTierId = user.Tier?.Id;

							if (userTierId != null)
							{
								// Retrieve the perks of the tier the user is tied to
								var perkList = _context.Perks.Where(p => p.TierId == userTierId).Include(p => p.Tier).ToList();

								foreach (var perk in perkList)
								{
									for (int i = 0; i < perk.VoucherQuantity; i++)
									{
										var voucher = new Voucher
										{
											DiscountExpiry = DateTime.Now.AddMonths(1),
											UserId = user.Id,
											PerkId = perk.Id,
											CreatedAt = DateTime.Now,
										};

										_context.Vouchers.Add(voucher);
									}

									_context.SaveChanges();
								}

							}

						}

						// Logs the thing if the voucher allocation is succesfull
						var logEntry = new AllocateVoucherLog
						{
							CreatedAt = DateTime.Now
						};
						_context.AllocateVoucherLog.Add(logEntry);
						_context.SaveChanges();
					}

					catch (Exception ex)
					{
						Console.WriteLine($"An error occurred during voucher allocation: {ex.Message}");
					}

				}
			}

			else
			{
				try 
				{
					var userList = _context.Users.Include(u => u.Tier).ToList();

					foreach (var user in userList)
					{
						// Retrieve the id of the tier the user is in, returns null if user as no tier (admin account)
						var userTierId = user.Tier?.Id;

						if (userTierId != null)
						{
							// Retrieve the perks of the tier the user is tied to
							var perkList = _context.Perks.Where(p => p.TierId == userTierId).Include(p => p.Tier).ToList();

							foreach (var perk in perkList)
							{
								for (int i = 0; i < perk.VoucherQuantity; i++)
								{
									var voucher = new Voucher
									{
										DiscountExpiry = DateTime.Now.AddMonths(1),
										UserId = user.Id,
										PerkId = perk.Id,
										CreatedAt = DateTime.Now,
									};

									_context.Vouchers.Add(voucher);
								}

								_context.SaveChanges();
							}

						}

					}

					// Logs the thing if the voucher allocation is succesfull
					var logEntry = new AllocateVoucherLog
					{
						CreatedAt = DateTime.Now
					};
					_context.AllocateVoucherLog.Add(logEntry);
					_context.SaveChanges();
				}

				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred during voucher allocation: {ex.Message}");
				}
			}


		}
	}
}
