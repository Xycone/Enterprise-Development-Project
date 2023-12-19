using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerkController : ControllerBase
    {
        private readonly MyDbContext _context;
        public PerkController(MyDbContext context)
        {
            _context = context;
        }

        // Accepts the tierId that the perk(s) u are looking for belongs to in the parameter
        [HttpGet]
        public IActionResult GetPerk(int tierid)
        {
            // Check if the specified tierid exists
            var existingTier = _context.Tiers.Find(tierid);

            if (existingTier == null)
            {
                return NotFound();
            }

            var perk = _context.Perks.Where(p => p.TierId == tierid).ToList();
            return Ok(perk);
        }

        // Accepts PercentageDiscount/FixedDiscount (only one of them can have a value), MinGroupSize, MinSpend, VoucherQuantity and TierId in the request body
        [HttpPost, Authorize(Roles = "admin")]
        public IActionResult AddPerk(Perk perk)
        {
            var now = DateTime.Now;

            // Checks if the provided TierId in the request body is valid
            var existingTier = _context.Tiers.Find(perk.TierId);
            if (existingTier == null) 
            {
                return NotFound();
            }

            // Check that either PercentageDiscount or FixedDiscount has a value
            if ((perk.PercentageDiscount != 0 && perk.FixedDiscount != 0) ||
                (perk.PercentageDiscount == 0 && perk.FixedDiscount == 0))
            {
                return BadRequest("The perk either provides a percentage discount or fixed discount voucher. Please choose one!");
            }

            var myPerk = new Perk()
            {
                PercentageDiscount = perk.PercentageDiscount,
                MinGroupSize = perk.MinGroupSize,
                MinSpend = perk.MinSpend,
                VoucherQuantity = perk.VoucherQuantity,
                TierId = perk.TierId,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.Perks.Add(myPerk);
            _context.SaveChanges();

            return Ok(myPerk);
        }

        // Accepts id of the perk that needs to be updated in the parameter
        // Accepts PercentageDiscount/FixedDiscount (only one of them can have a value), MinGroupSize, MinSpend and VoucherQuantity in the request body
        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public IActionResult UpdatePerk(int id, Perk perk)
        {
            var myPerk = _context.Perks.Find(id);
            if (myPerk == null) 
            {
                return NotFound();
            }

            // Validate that tierId is not present in the request body
            if (perk.TierId != myPerk.TierId)
            {
                return BadRequest("TierId cannot be changed in the update request.");
            }

            // Check that either PercentageDiscount or FixedDiscount has a value
            if ((perk.PercentageDiscount != 0 && perk.FixedDiscount != 0) ||
                (perk.PercentageDiscount == 0 && perk.FixedDiscount == 0))
            {
                return BadRequest("The perk either provides a percentage discount or fixed discount voucher. Please choose one!");
            }

            myPerk.PercentageDiscount = perk.PercentageDiscount;
            myPerk.FixedDiscount = perk.FixedDiscount;
            myPerk.MinGroupSize = perk.MinGroupSize;
            myPerk.MinSpend = perk.MinSpend;
            myPerk.VoucherQuantity = perk.VoucherQuantity;

            myPerk.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }

        // Accepts id of the perk that needs to be deleted in the parameter
        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public IActionResult DeletePerk(int id)
        {
            var myPerk = _context.Perks.Find(id);
            if (myPerk == null)
            {
                return NotFound();
            }
            _context.Perks.Remove(myPerk);
            _context.SaveChanges();
            return Ok();
        }



    }
}
