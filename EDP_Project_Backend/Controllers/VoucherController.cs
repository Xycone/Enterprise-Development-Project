using AutoMapper;
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VoucherController : ControllerBase                                     
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public VoucherController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Used to retrieve the authenticated user's userid
        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        // User

        // Retrieve all the vouchers (with voucher info) that belongs to the authorized user
        [HttpGet("GetMine"), Authorize]
        [ProducesResponseType(typeof(IEnumerable<VoucherDTO>), StatusCodes.Status200OK)]
        public IActionResult GetMine()
        {
            int userId = GetUserId();

            IQueryable<Voucher> result = _context.Vouchers.Include(v => v.User).Include(v => v.Perk).Where(v => v.UserId == userId);
            var list = result.OrderBy(x => x.CreatedAt).ToList();

            IEnumerable<VoucherDTO> data = list.Select(v => _mapper.Map<VoucherDTO>(v));
            return Ok(data);
        }

        // User uses/deletes the voucher
        // Takes in the voucher id of the voucher being used/deleted
        [HttpDelete("UseVoucher"), Authorize]
        public IActionResult UseVoucher(int id)
        {
            int userId = GetUserId();


            var myVoucher = _context.Vouchers.Find(id);
            // Checks if the voucher exists and if it does, ensure that the user can only delete/use up his/her own voucher
            if (myVoucher == null || myVoucher.UserId != userId)
            {
                return NotFound();
            }

            _context.Vouchers.Remove(myVoucher);
            _context.SaveChanges();
            return Ok();
        }

        // Admin

        // Retrieve all the vouchers availble in the db
        [HttpGet("GetAll"), Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(IEnumerable<VoucherDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            IQueryable<Voucher> result = _context.Vouchers.Include(v => v.User).Include(v => v.Perk);
            var list = result.OrderBy(x => x.CreatedAt).ToList();

            IEnumerable<VoucherDTO> data = list.Select(v => _mapper.Map<VoucherDTO>(v));
            return Ok(data);
        }

        // Admin can manually remove a voucher from the user 
        // Takes in the voucher id of the voucher being deleted
        [HttpDelete("DeleteVoucher"), Authorize(Roles = "admin")]
        public IActionResult YeetusDeletus(int id)
        {
            var myVoucher = _context.Vouchers.Find(id);
            // Checks if the voucher exists and if it does, ensure that the user can only delete/use up his/her own voucher
            if (myVoucher == null)
            {
                return NotFound();
            }

            _context.Vouchers.Remove(myVoucher);
            _context.SaveChanges();
            return Ok();
        }










    }
}
