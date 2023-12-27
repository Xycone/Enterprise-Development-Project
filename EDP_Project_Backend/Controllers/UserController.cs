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
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserController(MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
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
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check if the user is trying to log in as admin
            if (request.Email == "adminaccount@gmail.com" && request.Password == "Adm1nP@ssw0rd")
            {
                // Check if there's an existing admin user
                var existingAdmin = _context.Users.FirstOrDefault(x => x.IsAdmin);

                if (existingAdmin == null)
                {
                    // Create an admin account
                    var adminUser = new User
                    {
                        IsAdmin = true,
                        UserName = "Admin",
                        UserPassword = BCrypt.Net.BCrypt.HashPassword("Adm1nP@ssw0rd"),
                        UserEmail = "adminaccount@gmail.com"
                    };

                    _context.Users.Add(adminUser);
                    _context.SaveChanges();
                }
            }

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

            // Return user info
            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);
            string accessToken = CreateToken(foundUser);
            LoginResponse response = new()
            {
                User = userDTO,
                AccessToken = accessToken
            };

            return Ok(new { user, accessToken });
        }

        // Used to retrieve user info stored in claims
        [HttpGet("auth"), Authorize]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public IActionResult Auth()
        {
            var id = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
            var isAdmin = User.IsInRole("admin");

            if (id != 0 && name != null && email != null)
            {
                UserDTO userDTO = new() { Id = id, UserName = name, UserEmail = email, IsAdmin = isAdmin };
                AuthResponse response = new() { User = userDTO };
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        // Used by users to retrieve thier own user data
        // Might be adding more in the future such as the url that points to the user profile picture resource
        [HttpGet("profile"), Authorize]
        [ProducesResponseType(typeof(IEnumerable<UserProfileDTO>), StatusCodes.Status200OK)]
        public IActionResult GetUserProfile()
        {
            int userId = GetUserId();
            var user = _context.Users.Include(u => u.Tier).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var data = _mapper.Map<UserProfileDTO>(user);

            return Ok(data);
        }

        // Returns all the users in the db
        [HttpGet("view-users"), Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(IEnumerable<UserProfileDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string? search)
        {
            int userId = GetUserId();
            IQueryable<User> result = _context.Users.Where(x => x.Id != userId);
            if (search != null)
            {
                result = result.Where(x => x.UserName.Contains(search));
            }

            var userList = result
                .OrderBy(x => x.UserName)
                .Select(user => new UserViewDTO
                {
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserHp = user.UserHp,
                    TierName = _context.Tiers.Where(t => t.Id == user.TierId).Select(t => t.TierName).FirstOrDefault()
                }).ToList();

            return Ok(userList);
        }

        // For users to update their own profile
        // Takes in UserName, UserEmail and UserHp in the request body
        [HttpPut("update-profile"), Authorize]
        public IActionResult UpdateUser(UpdateProfileRequest user)
        {
            int userId = GetUserId();
            var myUser = _context.Users.Find(userId);
            if (myUser == null)
            {
                return NotFound();
            }      


            if (user.UserName != null) 
            {
                myUser.UserName = user.UserName.Trim();
            }

            if (user.UserPassword != null)
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.UserPassword.Trim());
                myUser.UserPassword = passwordHash;
            }

            if (user.UserEmail != null)
            {
                // Check if updated email already exists
                var foundUser = _context.Users.Where(x => x.UserEmail == user.UserEmail && x.Id != userId).FirstOrDefault();
                if (foundUser != null)
                {
                    string message = "Email already exists.";
                    return BadRequest(new { message });
                }

                myUser.UserEmail = user.UserEmail.Trim().ToLower();
            }

            if (user.UserHp != null)
            {
                myUser.UserHp = user.UserHp.Trim();
            }
            
            myUser.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Ok(myUser);

        }





    }
}
