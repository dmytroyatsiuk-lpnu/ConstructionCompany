using ConstructionCompany.Models.ModelsDTO;
using ConstructionCompany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConstructionCompany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ConstructionCompanyDbContext _context;

        public AuthController(ConstructionCompanyDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);

            if (user == null || user.PasswordHash != request.Password)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(user.Username, user.Role);
            return Ok(new
            {
                token,
                role = user.Role
            });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var key = Encoding.UTF8.GetBytes("ONvVE0bKYIfyQH8wASR9C41lTqtPo2JjXofAY4l2q1KdsbE6J7QWFh8iUBj3CP0yKfUbqFo3J1vnXxIlDcr0dCsOTpQB9A6PqReQb8AxZhjwJGflXcT4sEN5WvFU6DiPSB8wcMAQaIKflxE9mYr6btNy4T1kDdZ3");
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: "ConstructionCompanyAPI",
                audience: "ConstructionCompanyClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
