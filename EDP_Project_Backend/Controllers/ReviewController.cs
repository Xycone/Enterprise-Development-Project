using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController: ControllerBase
    {
        private readonly MyDbContext _context;
        public ReviewController(MyDbContext context)
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

			// Return a default value (e.g., 2) if the user ID is not available or not valid.
			return 1;
		}

		[HttpGet]
        public IActionResult GetAll()
        {
            IQueryable<Review> result = _context.Reviews;
            var list = result.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var review = _context.Reviews.FirstOrDefault(x => x.Id == id);
			if (review == null)
			{
				return NotFound(); // Return 404 if review with the specified ID is not found
			}
			return Ok(review);
		}


		[HttpPost]
        public IActionResult AddReview(Review review)
        {
            var thisReview = new Review
            {
                Date = review.Date,
                Id = review.Id,
                ActivityId = review.ActivityId,
                starRating = review.starRating,
                Desc = review.Desc,
                UserName = review.UserName,
                UserId = GetUserId(),
			};

            _context.Reviews.Add(thisReview);
            _context.SaveChanges();
            return Ok(thisReview);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReview(int id, Review review) 
        {
            var now = DateTime.Now;
            var thisReview = _context.Reviews.Find(id);
            if (thisReview == null)
            {
                return NotFound();
            }
            thisReview.Date = now;
            thisReview.starRating = review.starRating;
            thisReview.Desc = review.Desc;
            _context.Reviews.Add(thisReview);
            _context.SaveChanges();
            return Ok(thisReview);


        }


        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var thisReview = _context.Reviews.Find(id);
            if (thisReview == null)
            {
                return NotFound();
            }
            _context.Reviews.Remove(thisReview);
            _context.SaveChanges();
            return Ok();
        }
    }
}
