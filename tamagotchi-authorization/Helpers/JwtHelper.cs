using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace tamagotchi_authorization.Helpers
{
    public class JwtHelper
    {
        public static string GenerateToken(int userId)
        {
            var claims = new[] { new Claim("user_id", userId.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Scope.SecurityKey));
            var signInCredential = new SigningCredentials(key, "HS256");
            var dateNow = DateTime.UtcNow;
            var token = new JwtSecurityToken(              
                notBefore: dateNow,
                expires: dateNow.Add(TimeSpan.FromHours(Scope.LifeTime)),
                claims: claims,
                signingCredentials: signInCredential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
