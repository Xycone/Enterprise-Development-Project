using AutoMapper;
using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityListingController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public ActivityListingController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivityListing([FromBody] AddListingRequest createListing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var activityListing = new ActivityListing
                {
                    Name = createListing.Name,
                    Address = createListing.Address,
                    Category = createListing.Category,
                    Description = createListing.Description,
                    Nprice = createListing.Nprice,
                    Capacity = createListing.Capacity,
                };
                Console.WriteLine(activityListing);
                _context.ActivityListings.Add(activityListing);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetActivityListing), new { id = activityListing.Id }, activityListing);
            }
            catch (Exception)
            {
                // Handle exceptions appropriately (e.g., logging, returning error response)
                return StatusCode(500, "Internal server error");
            }
        }

        // The GetActivityListing method remains the same as in the previous example
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ListingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivityListing(int id)
        {
            try
            {
                var listing = await _context.ActivityListings.FindAsync(id);

                if (listing == null)
                {
                    return NotFound();
                }

                var listingDTO = _mapper.Map<ListingDTO>(listing);

                return Ok(listingDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("listings")]
        [ProducesResponseType(typeof(IEnumerable<ListingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActivityListings()
        {
            try
            {
                var activityListings = await _context.ActivityListings.ToListAsync();

                var listingDTOs = activityListings.Select(listing => _mapper.Map<ListingDTO>(listing));

                return Ok(listingDTOs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UpdateListingRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateActivityListing(int id, [FromBody] UpdateListingRequest updatedListingRequest)
        {
            try
            {
                var listingToUpdate = await _context.ActivityListings.FindAsync(id);

                if (listingToUpdate == null)
                {
                    return NotFound();
                }

                // Update the properties from the request model to the entity
                listingToUpdate.Name = updatedListingRequest.Name;
                listingToUpdate.Address = updatedListingRequest.Address;
                listingToUpdate.Category = updatedListingRequest.Category;
                listingToUpdate.Description = updatedListingRequest.Description;
                listingToUpdate.Nprice = updatedListingRequest.Nprice;
                listingToUpdate.Capacity = updatedListingRequest.Capacity;

                _context.Entry(listingToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var updatedListing = _mapper.Map<ListingDTO>(listingToUpdate);

                return Ok(updatedListing);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteActivityListing(int id)
        {
            try
            {
                var listingToDelete = await _context.ActivityListings.FindAsync(id);

                if (listingToDelete == null)
                {
                    return NotFound();
                }

                _context.ActivityListings.Remove(listingToDelete);
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
