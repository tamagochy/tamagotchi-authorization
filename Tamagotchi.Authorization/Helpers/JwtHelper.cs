using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tamagotchi.Authorization.Helpers
{
    public class JwtHelper
    {
        public static string GenerateToken(int userId, string secretKey, int lifeTime)
        {
            var claims = new[] { new Claim("user_id", $"{userId}", ClaimValueTypes.Integer) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signInCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var dateNow = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: "tama.gotchi",
                audience: "tama.gotchi",
                notBefore: dateNow,
                expires: dateNow.Add(TimeSpan.FromHours(lifeTime)),
                claims: claims,
                signingCredentials: signInCredential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
