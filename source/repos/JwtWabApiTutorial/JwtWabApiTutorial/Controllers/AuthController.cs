using JwtWabApiTutorial.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtWabApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AuthController(IConfiguration configuration , DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var getuser = _context.Users.SingleOrDefault(x => x.Username == request.Username);
            if (getuser == null)
            {
                return BadRequest("User not found");

            }
            if (!VerifyPasswordHash( getuser.Password, request.Password))
            {
                return BadRequest("Wrong password.");
            }
            string token = CreateToken(getuser);
             Response.Cookies.Append("token", token, new CookieOptions()
            {
                HttpOnly = true
            });

            return Ok(token);
        }
        [HttpGet("Users")]
        public ActionResult<List<User>> GetUsers()
        {
            return  _context.Users.ToList();
        }
        [HttpGet("User")]
        public User GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new User
                {
                    Username = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    Role = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };
            }
            return null;



        }
        [HttpPost("logout")]
        public ActionResult Logout(UserDto request)
        {
            Response.Cookies.Delete("token");
            return Ok(new
            {
                message = "success"
            });
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
           
            return jwt;
        }
        private bool VerifyPasswordHash(string password, string passwordHash)
        {

                return password.Equals(passwordHash);
            
        }
        private JwtSecurityToken VerifyToken(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value);

            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            },out SecurityToken validatedToken);

            return (JwtSecurityToken) validatedToken;

        }


    }
}
