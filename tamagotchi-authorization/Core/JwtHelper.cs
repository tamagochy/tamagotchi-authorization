using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace tamagotchi_authorization.Core
{
    public class JwtHelper
    {
        public static string GenerateToken(string username)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Scope.SecurityKey));
            var signInCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var dateNow = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: Scope.BaseURL,
                audience: Scope.BaseURL,
                notBefore: dateNow,
                expires: dateNow.Add(TimeSpan.FromHours(Scope.LifeTime)),
                claims: claims,
                signingCredentials: signInCredential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
