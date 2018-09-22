using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace tamagotchi_authorization.Controllers
{
    [Route("/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("token")]
        public IActionResult Token()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "userName") };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TamagochiSecretKey"));
            var signInCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: "tamagochi.com",
                audience: "tamagochi.com",
                expires: DateTime.Now.AddMinutes(1),
                claims: claims,
                signingCredentials: signInCredential
                );
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
