using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using tamagotchi_authorization.Models;
using tamagotchi_authorization.Core;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace tamagotchi_authorization.Controllers
{
    [Route("/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("token")]
        public IActionResult Login([FromBody]JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            if (login == null || password == null)
                return BadRequest("Wrong parameters");
            var user = new UserRepository().GetUser(login);
            if (user == null)
                return NotFound();
            var credentials = user.Password.Equals(password);
            if (!credentials)
                return BadRequest("Auth failed");
            return Ok(JwtHelper.GenerateToken(login));
        }
    }
}
