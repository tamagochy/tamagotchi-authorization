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
            var header = Request.Headers["Authorization"].ToString();
            if (header.StartsWith("Basic"))
            {
                var credentialValue = header.Substring("Basic ".Length).Trim();
                var userNameAndPassEnc = Encoding.UTF8.GetString(Convert.FromBase64String(credentialValue)); // admin:pass
                var userNameAndPass = userNameAndPassEnc.Split(":");
                // TODO: check DB username and pass
                //test
                if (userNameAndPass[0] == "Admin" && userNameAndPass[1] == "pass")
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, userNameAndPass[0]) };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Scope.SecurityKey));
                    var signInCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                    var token = new JwtSecurityToken(
                        issuer: Scope.BaseURL,
                        audience: Scope.BaseURL,
                        expires: DateTime.Now.AddMinutes(1),
                        claims: claims,
                        signingCredentials: signInCredential
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                    return BadRequest("Auth failed");
                
            }
            return BadRequest("Wrong request");

        }
    }
}
