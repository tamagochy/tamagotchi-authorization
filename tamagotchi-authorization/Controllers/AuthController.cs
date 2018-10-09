using Microsoft.AspNetCore.Mvc;
using System;
using tamagotchi_authorization.Core;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

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

        #region Access Recovery

        [HttpPost("send_page_access")]
        public IActionResult SendMailWithPageAccess([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var pageAccessAddress = (string)jsonBody["pageAccess"];
            if (login == null || pageAccessAddress == null)
                return BadRequest("Отсутсвует один из параметров запроса (Internal error).");
            var user = _userRepository.GetUser(login);
            if (user == null)
                return BadRequest("Пользователь отсутствует в системе.");
            try
            {
                SendEmailAsync(user.Email, pageAccessAddress).GetAwaiter();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            return Ok();
        }

        private static async Task SendEmailAsync(string userMail, string pageAccess)
        {
            var from = new MailAddress(Scope.ApplicationMail);
            var to = new MailAddress(userMail);
            var message = new MailMessage(from, to)
            {
                Subject = "Восстановление доступа",
                Body = "Вы хотите восстановить пароль доступа.\nПожалуйста, посетите страницу восстановления:\n" + pageAccess 
            };
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(Scope.ApplicationMail, Scope.MailPassword),
                EnableSsl = true
            };
            await smtp.SendMailAsync(message);
        }

        [HttpPost("password_recovery")]
        public IActionResult RecoveryPassword([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var newPassword = (string)jsonBody["newPassword"];
            if (login == null || newPassword == null)
                return BadRequest("Отсутсвует один из параметров запроса (Internal error).");
            var user = _userRepository.GetUser(login);
            if (user == null)
                return BadRequest("Пользователь отсутствует в системе.");
            //TODO: update user password in db 
            //_userRepository.Entry(user).State = EntityState.Modified;
            //user.Password = newPassword;
            //_userRepository.SaveChanges();
            return Ok();
        }

        #endregion
    }
}
