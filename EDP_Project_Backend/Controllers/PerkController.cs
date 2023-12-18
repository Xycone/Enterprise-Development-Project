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

        [HttpPost, Authorize(Roles = "admin")]
        public IActionResult AddPerk(Perk perk)
        {
            var now = DateTime.Now;

            // Checks if the provided TierId in the request body is valid
            var existingTier = _context.Tiers.Find(perk.TierId);
            if (existingTier == null) 
            {
                return BadRequest("Perk id does not exists.");
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



    }
}
