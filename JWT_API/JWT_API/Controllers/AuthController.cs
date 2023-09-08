using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        public AuthController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromQuery] LoginRequest loginRequest) 
        {
            var claim = new[] { 
                new Claim(JwtRegisteredClaimNames.Aud, "MyApp_A"),   // Setting the audience to "MyApp_A"
                new Claim(JwtRegisteredClaimNames.Sub, loginRequest.username), // Claim for the subject (usually the user ID or username)
            };

            var token = "";
            if (loginRequest.username == "san" && loginRequest.password == "san123")
            {
                token = GenerateJwtToken(claim);
            }
            else
            {
                token = "Incorrect username or password!";
            }
            return Ok(new { token });
        }

        private string GenerateJwtToken(Claim[] claims)
        {
            var SSKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    claims: claims,
                    //expires: DateTime.Now.AddMinutes(30), //expire token in 30 mins
                    signingCredentials: new SigningCredentials(SSKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
