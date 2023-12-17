using EDP_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        private string CreateToken(User user)
        {
            string secret = _configuration.GetValue<string>("Authentication:Secret");
            int tokenExpiresDays = _configuration.GetValue<int>("Authentication:TokenExpiresDays");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                // If user IsAdmin is true, it assigns the value "admin" to the role claim.
                // If user isAdmin is false, it assigns the value "user" to the role claim.
                new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user")
            }),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {

            // Trim string values
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            request.PhoneNumber = request.PhoneNumber.Trim();

            // Check email
            var foundUser = _context.Users.Where(x => x.UserEmail == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            // Checks for the tier with the lowest tier position
            var lowestTier = _context.Tiers.OrderBy(t => t.TierPosition).FirstOrDefault();
            if (lowestTier == null)
            {
                return BadRequest("No tiers available. Unable to assign user to a tier.");
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                UserName = request.Name,
                UserEmail = request.Email,
                UserPassword = passwordHash,
                IsAdmin = false,
                UserHp = request.PhoneNumber,
                TierId = lowestTier.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            // Check email and password
            string message = "Email or password is not correct.";
            var foundUser = _context.Users.Where(
            x => x.UserEmail == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(
            request.Password, foundUser.UserPassword);
            if (!verified)
            {
                return BadRequest(new { message });
            }
            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.UserEmail,
                foundUser.UserName,
                foundUser.UserHp,
                foundUser.TotalSpent,
                foundUser.TotalBookings,
                foundUser.IsAdmin
            };

            string accessToken = CreateToken(foundUser);
            return Ok(new { user, accessToken });
        }

        // Used to retrieve info stored in claims
        // Not sure if i would really be using this tbh
        [HttpGet("auth"), Authorize]
        public IActionResult Auth()
        {
            var id = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
            var isAdmin = User.IsInRole("admin");

            if (id != 0 && name != null && email != null)
            {
                var user = new
                {
                    id,
                    email,
                    name,
                    isAdmin
                };
                return Ok(new { user });
            }
            else
            {
                return Unauthorized();
            }
        }

        // Used by users to retrieve thier own user data
        [HttpGet("profile"), Authorize]
        public IActionResult GetUserProfile()
        {
            int userId = GetUserId();
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var tier = _context.Tiers.FirstOrDefault(t => t.Id == user.TierId);

            var userData = new
            {
                user.Id,
                user.UserName,
                user.UserEmail,
                user.UserHp,
                user.TotalSpent,
                user.TotalBookings,
                // Theorethically will show null if cant find a tier associated to the user
                // Shd not happen tho as a user will be assigned the tier with the lowest tier position on registration
                tier?.TierName
            };

            return Ok(userData);
        }

        // Called after checkout is confirmed to check if there shd be any upgrades in tier
        [HttpPut("update-tier"), Authorize]
        public IActionResult UpdateUserTier()
        {
            int userId = GetUserId();  // Assuming you have a method to get the user ID

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Retrieve the tier associated with the user
            var userTier = _context.Tiers.FirstOrDefault(t => t.Id == user.TierId);
            if (userTier == null)
            {
                return BadRequest("User is not associated with a tier.");
            }

            // Checks conditions for tier upgrade based on events booked and money spent
            if (user.TotalBookings >= userTier.TierBookings && user.TotalSpent >= userTier.TierSpendings)
            {
                // Performs tier upgrade operation
                var nextTier = _context.Tiers.FirstOrDefault(t => t.TierPosition == userTier.TierPosition + 1);
                if (nextTier != null)
                {
                    // Increase the user tier by 1
                    // Subtract the overflow bookings by the amt used to upgrade the tier
                    // Subtract the overflow spendings by the amt used to upgrade the tier
                    user.TierId = nextTier.Id;
                    user.TotalBookings -= userTier.TierBookings;
                    user.TotalSpent -= userTier.TierSpendings;

                    _context.SaveChanges();
                    return Ok("User tier upgraded successfully.");
                }
                else
                {
                    return BadRequest("No higher tier available for upgrade.");
                }

            }
            else
            {
                return BadRequest("User does not meet the criteria for tier upgrade");
            }
        }

    }
}
