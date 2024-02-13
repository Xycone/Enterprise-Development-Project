using AutoMapper;
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NoticeController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public NoticeController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NoticeDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAllNotices()
        {
            var notices = _context.Notices.ToList();
            var noticeDTOs = _mapper.Map<List<NoticeDTO>>(notices);
            return Ok(noticeDTOs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NoticeDTO), StatusCodes.Status200OK)]
        public IActionResult GetNoticeById(int id)
        {
            var notice = _context.Notices.FirstOrDefault(n => n.Id == id);

            if (notice == null)
            {
                return NotFound($"Notice with ID {id} not found.");
            }

            var noticeDTO = _mapper.Map<NoticeDTO>(notice);

            return Ok(noticeDTO);
        }

        [HttpPost("AddNotice")]
        [ProducesResponseType(typeof(NoticeDTO), StatusCodes.Status200OK)]
        public IActionResult AddNotice(AddNoticeRequest noticeRequest)
        {
            try
            {
                // Create a new notice entity
                var newNotice = new Notice
                {
                    Name = noticeRequest.Name.Trim(),
                    Description = noticeRequest.Description.Trim(),
                    // Additional properties specific to your Notice entity
                };

                // Add the new notice to the database
                _context.Notices.Add(newNotice);
                _context.SaveChanges();

                // Map the notice entity to a DTO for response
                NoticeDTO newNoticeDTO = _mapper.Map<NoticeDTO>(newNotice);

                return Ok(newNoticeDTO);
            }
            catch (Exception ex)
            {
                // Log the error and return a 500 status code
                return StatusCode(500);
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateNotice(int id, UpdateNoticeRequest noticeRequest)
        {
            try
            {
                var myNotice = _context.Notices.Find(id);
                if (myNotice == null)
                {
                    return NotFound();
                }

                // Update notice properties if provided in the request
                if (noticeRequest.Name != null)
                {
                    myNotice.Name = noticeRequest.Name.Trim();
                }
                if (noticeRequest.Description != null)
                {
                    myNotice.Description = noticeRequest.Description.Trim();
                }

                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteNotice(int id)
        {
            var notice = _context.Notices.FirstOrDefault(n => n.Id == id);

            if (notice == null)
            {
                return NotFound($"Notice with ID {id} not found.");
            }

            _context.Notices.Remove(notice);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
