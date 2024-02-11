using AutoMapper;
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public ActivityController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("{listingId}")]
        public async Task<IActionResult> CreateActivity(int listingId, [FromBody] AddActivityRequest addActivityRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var activityListing = await _context.ActivityListings.FindAsync(listingId);

                if (activityListing == null)
                {
                    return NotFound("Activity Listing not found");
                }

                var activity = new Activity
                {
                    Date = addActivityRequest.Date,
                    AvailSpots = addActivityRequest.AvailSpots,
                    ActivityListingId = listingId
                };

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();

                return Ok("Activity created and added to the listing successfully");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ActivityDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivity(int id)
        {
            try
            {
                var activity = await _context.Activities.FindAsync(id);

                if (activity == null)
                {
                    return NotFound();
                }

                var activityDTO = _mapper.Map<ActivityDTO>(activity);

                return Ok(activityDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ActivityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActivities()
        {
            try
            {
                var activities = await _context.Activities.ToListAsync();

                var activityDTOs = _mapper.Map<List<ActivityDTO>>(activities);

                return Ok(activityDTOs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("activities-by-listing/{listingId}")]
        [ProducesResponseType(typeof(IEnumerable<ActivityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetActivitiesByListing(int listingId)
        {
            try
            {
                var activities = _context.Activities
                    .Where(a => a.ActivityListingId == listingId)
                    .Select(a => new ActivityDTO
                    {
                        Id = a.Id,
                        Date = a.Date,
                        AvailSpots = a.AvailSpots,
                        ActivityListingId = a.ActivityListingId,
                    })
                    .ToList();

                return Ok(activities);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UpdateActivityRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityRequest updateActivityRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingActivity = await _context.Activities.FindAsync(id);

                if (existingActivity == null)
                {
                    return NotFound("Activity not found");
                }

                // Update properties from the request model
                existingActivity.Date = updateActivityRequest.Date;
                existingActivity.AvailSpots = updateActivityRequest.AvailSpots;


                _context.Activities.Update(existingActivity);
                await _context.SaveChangesAsync();

                return Ok("Activity updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            try
            {
                var activityToDelete = await _context.Activities.FindAsync(id);

                if (activityToDelete == null)
                {
                    return NotFound();
                }

                _context.Activities.Remove(activityToDelete);
                await _context.SaveChangesAsync();

                return NoContent(); // Successfully deleted
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
