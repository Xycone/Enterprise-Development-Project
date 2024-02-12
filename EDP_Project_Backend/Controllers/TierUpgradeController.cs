using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// TODO: Find some way to ensure that the update-bookings and update-spendings can only be called after payment is confirmed and successful on STRIPE
// As of right now the request can be made as long as the user is authorized and knows the API endpoint which we do not want happening


// Controller used to store APIs that is used in the upgrading of the user's tier

// Hmm apparently it might be better to not put the code that does not require admin or user intervention as an API request
// Will just leave this here for future use if i ever decide to use it
// To be deleted on project submission
namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TierUpgradeController : ControllerBase
    {
        private readonly MyDbContext _context;
        public TierUpgradeController(MyDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        // Called after update-spendings and update-bookings is called to check if there shd be any upgrades in tier
        [HttpPut("update-tier"), Authorize]
        public IActionResult UpdateUserTier(int userId)
        {
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Retrieve the tier associated with the user
            var userTier = _context.Tiers.FirstOrDefault(t => t.Id == user.TierId);
            if (userTier == null)
            {
                return BadRequest("User is not associated with a tier.");
            }

            // Checks conditions for tier upgrade based on events booked and money spent
            if (user.TotalBookings >= userTier.TierBookings && user.TotalSpent >= userTier.TierSpendings)
            {
                // Performs tier upgrade operation
                var nextTier = _context.Tiers.FirstOrDefault(t => t.TierPosition == userTier.TierPosition + 1);
                if (nextTier != null)
                {
                    // Increase the user tier by 1
                    // Subtract the overflow bookings by the amt used to upgrade the tier
                    // Subtract the overflow spendings by the amt used to upgrade the tier
                    user.TierId = nextTier.Id;
                    user.TotalBookings -= userTier.TierBookings;
                    user.TotalSpent -= userTier.TierSpendings;

                    _context.SaveChanges();
                    return Ok(true);
                }
                else
                {
                    return BadRequest(false);
                }

            }
            else
            {
                return BadRequest(false);
            }
        }
    }
}
