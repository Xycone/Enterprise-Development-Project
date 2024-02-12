using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketController: ControllerBase
    {
        private readonly MyDbContext _context;
        public TicketController(MyDbContext context)
        {
            _context = context;
        }
		private int GetUserId()
		{
			try
			{
				var userIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
				if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
				{
					return userId;
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				Console.WriteLine($"Error in GetUserId: {ex.Message}");
			}
			return 0;
		}
		[HttpGet]
        public IActionResult GetAll()
        {
            IQueryable<Ticket> result = _context.Tickets;
            var list = result.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }
        [HttpPost]
        public IActionResult AddTicket(Ticket ticket)
        {
            var now = DateTime.Now;

            var thisTicket = new Ticket()
            {
                Id = ticket.Id,
                UserId = GetUserId(),
                Date = now,
                IssueType = ticket.IssueType,
                Complaint = ticket.Complaint,
                Contact = ticket.Contact,
 
            };
            _context.Tickets.Add(thisTicket);
            _context.SaveChanges();
            return Ok(thisTicket);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTicket(int id, Ticket ticket)
        {
            var thisTicket = _context.Tickets.Find(id);
            if (thisTicket == null)
            {
                return NotFound();
            }
            thisTicket.Date = DateTime.Now;
            thisTicket.IssueType = ticket.IssueType.Trim();
            thisTicket.Complaint = ticket.Complaint.Trim();
            thisTicket.Contact = ticket.Contact.Trim();
            _context.SaveChanges();
            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var thisTicket = _context.Tickets.Find(id);
            if (thisTicket == null)
            {
                return NotFound();
            }
            _context.Tickets.Remove(thisTicket);
            _context.SaveChanges();
            return Ok();
        }

    }
}
