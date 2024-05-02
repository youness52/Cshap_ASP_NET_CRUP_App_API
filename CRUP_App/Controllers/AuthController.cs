using CRUP_App.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CRUP_App.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DotnetdbContext _dotnetdbContext;
        private readonly IConfiguration _configuration;

        public AuthController(DotnetdbContext dotnetdbContext, IConfiguration configuration)
        {
            _dotnetdbContext = dotnetdbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post(User _userData)
        {
            if (_userData == null || string.IsNullOrWhiteSpace(_userData.Username) || string.IsNullOrWhiteSpace(_userData.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await GetUser(_userData.Username, _userData.Password);

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }
         

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim("Id", user.Id.ToString()),
                        new Claim("FirstName", user.FirstName),
                        new Claim("LastName", user.LastName),
                        new Claim("Username", user.Username),
                }),
          
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);


            //return Ok(new JwtSecurityTokenHandler().WriteToken(token));

            var response = new
            {
                Token = tokenHandler.WriteToken(token),
                UserInfo = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Username // Add any additional user information you want to include
                }
            };

            return Ok(response);
        }

        private async Task<User> GetUser(string username, string password)
        {
             password = CalculateMD5Hash(password);
            return await _dotnetdbContext.User.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
        private string CalculateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        [HttpGet("userinfo")]
        [Authorize] // Requires authentication
        public IActionResult GetUserInfo()
        {
            // Retrieve user claims
            var userIdClaim = User.FindFirst("Id"); // Get user ID claim
            var firstNameClaim = User.FindFirst("FirstName"); // Get first name claim
            var lastNameClaim = User.FindFirst("LastName"); // Get last name claim
            var usernameClaim = User.FindFirst("Username"); // Get username claim

            // Return user information
            return Ok(new
            {
                UserId = userIdClaim?.Value,
                FirstName = firstNameClaim?.Value,
                LastName = lastNameClaim?.Value,
                Username = usernameClaim?.Value
            });
        }
    }
}
