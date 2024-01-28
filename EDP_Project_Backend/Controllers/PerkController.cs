using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerkController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public PerkController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Accepts the tierId that the perk(s) u are looking for belongs to in the parameter
        [HttpGet("{tierid}")]
        [ProducesResponseType(typeof(IEnumerable<PerkDTO>), StatusCodes.Status200OK)]
        public IActionResult GetPerks(int tierid)
        {

            var perks = _mapper.Map<List<PerkDTO>>(_context.Perks.Where(p => p.TierId == tierid).Include(p => p.Tier).ToList());

            return Ok(perks);
        }

        // Accepts PercentageDiscount/FixedDiscount (only one of them can have a value), MinGroupSize, MinSpend, VoucherQuantity and TierId in the request body
        [HttpPost, Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(PerkDTO), StatusCodes.Status200OK)]
        public IActionResult AddPerk(AddPerkRequest perk)
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
                FixedDiscount = perk.FixedDiscount,
                MinGroupSize = perk.MinGroupSize,
                MinSpend = perk.MinSpend,
                VoucherQuantity = perk.VoucherQuantity,
                TierId = perk.TierId,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.Perks.Add(myPerk);
            _context.SaveChanges();


            Perk? newPerk = _context.Perks.FirstOrDefault(p => p.Id == myPerk.Id);
            PerkDTO perkDTO = _mapper.Map<PerkDTO>(newPerk);
            return Ok(perkDTO);
        }

        // Accepts id of the perk that needs to be updated in the parameter
        // Accepts PercentageDiscount/FixedDiscount (only one of them can have a value), MinGroupSize, MinSpend and VoucherQuantity in the request body
        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public IActionResult UpdatePerk(int id, UpdatePerkRequest perk)
        {
            var myPerk = _context.Perks.Find(id);
            if (myPerk == null) 
            {
                return NotFound();
            }

            // Check that either PercentageDiscount or FixedDiscount has a value
            if ((perk.PercentageDiscount != 0 && perk.FixedDiscount != 0) || (perk.PercentageDiscount == 0 && perk.FixedDiscount == 0))
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
