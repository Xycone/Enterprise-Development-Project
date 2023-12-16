using EDP_Project_Backend.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TierController : ControllerBase
    {
        private readonly MyDbContext _context;
        public TierController(MyDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Tier> result = _context.Tiers;
            if (search != null)
            {
                result = result.Where(x => x.TierName.Contains(search));
            }
            var list = result.OrderBy(x => x.TierPosition).ToList();
            return Ok(list);
        }


        // Fields needed TierName, TierBookings and TierSpendings
        [HttpPost]
        public IActionResult AddTier(Tier tier)
        {
            var now = DateTime.Now;
            // Retrieves the current highest tier position in the db. 
            // If nothing in the db, the currentHighestTier will be given the null value
            int? currentHighestTier = _context.Tiers.Max(t => (int?)t.TierPosition);

            // By default, newly created tier will, be in the last tier position
            var myTier = new Tier()
            {
                TierName = tier.TierName.Trim(),
                TierBookings = tier.TierBookings,
                TierSpendings = tier.TierSpendings,
                // if currentHighestTier is null the currentHighestTier will be given a default value of 0
                TierPosition = (currentHighestTier ?? 0) + 1,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.Tiers.Add(myTier);
            _context.SaveChanges();
            return Ok(myTier);
        }


        [HttpGet("{id}")]
        public IActionResult GetTier(int id)
        {
            Tier? tier = _context.Tiers.Find(id);
            if (tier == null)
            {
                return NotFound();
            }
            return Ok(tier);
        }


        // Fields needed TierName, TierBookings, TierSpendings and TierPosition
        [HttpPut("{id}")]
        public IActionResult UpdateTier(int id, Tier tier)
        {
            var myTier = _context.Tiers.Find(id);
            if (myTier == null)
            {
                return NotFound();
            }

            // Prevents the updated tierposition from being greater than the number of 
            if (tier.TierPosition > _context.Tiers.Count())
            {
                return BadRequest("Invalid TierPosition. Tier position cannot be greater than the number of tiers");
            }

            myTier.TierName = tier.TierName.Trim();
            myTier.TierBookings = tier.TierBookings;
            myTier.TierSpendings = tier.TierSpendings;

            // Check if TierPosition is being changed in this update request
            if (myTier.TierPosition != tier.TierPosition)
            {

                // New TierPosition of the tier
                var newPosition = tier.TierPosition;

                if (newPosition < myTier.TierPosition)
                {
                    // If the newPosition is lower than the original position (e.g. 3 move to 1),
                    // the other tiers with tier position greater than or equal to the new position will be moved back by one to make space
                    var affectedTiersLower = _context.Tiers.Where(t => t.TierPosition >= newPosition && t.TierPosition < myTier.TierPosition && t.Id != myTier.Id).ToList();
                    foreach (var affectedTier in affectedTiersLower)
                    {
                        affectedTier.TierPosition++;
                    }
                }
                else
                {
                    // If the newPosition is higher than the original position (e.g. 1 move to 3),
                    // the other tiers with tier position greater than or equal to the new position will be moved forward by one to fill the gap
                    var affectedTiersHigher = _context.Tiers.Where(t => t.TierPosition > myTier.TierPosition && t.TierPosition <= newPosition && t.Id != myTier.Id).ToList();
                    foreach (var affectedTier in affectedTiersHigher)
                    {
                        affectedTier.TierPosition--;
                    }
                }

                myTier.TierPosition = newPosition;
            }

            myTier.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTier(int id)
        {
            var myTier = _context.Tiers.Find(id);
            if (myTier == null)
            {
                return NotFound();
            }
            _context.Tiers.Remove(myTier);
            _context.SaveChanges();
            return Ok();
        }

    }
}
