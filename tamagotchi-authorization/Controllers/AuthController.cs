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
        private UserRepository _userRepository;
        public AuthController()
        {
            _userRepository = new UserRepository();
        }
        [HttpPost("token")]
        public IActionResult Login([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            if (login == null || password == null)
                return BadRequest("Отсутсвует один из параметров запроса (Internal error).");
            var user = _userRepository.GetUser(login);
            if (user == null)
                return NotFound("Логин не найден в системе.");
            var credentials = user.Password.Equals(password);
            if (!credentials)
                return BadRequest("Ошибка авторизации. Неверный пароль.");
            return Ok(JwtHelper.GenerateToken(login));
        }

        [HttpPost("registration")]
        public IActionResult Registration([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            var passwordConfirmation = (string)jsonBody["passwordConfirm"];
            var email = (string)jsonBody["email"];
            if (login == null || password == null || passwordConfirmation == null || email == null)
                return BadRequest("Отсутсвует один из параметров запроса (Internal error).");
            if (!password.Equals(passwordConfirmation))
                return BadRequest("Пароли не совпадают.");
            if (_userRepository.GetUserByEMail(email) != null)
                return BadRequest("Пользователь с указанным адресом электронной почты уже существует.");
            if (_userRepository.GetUser(login) != null)
                return BadRequest("Указанный логин уже существует, попробуйте использовать другой.");
            //TODO: add user to db
            return Ok();
        }
    }
}
