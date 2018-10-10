using Microsoft.AspNetCore.Mvc;
using System;
using tamagotchi_authorization.Helpers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using tamagotchi_authorization.Core;

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
        [HttpPost("Token")]
        public ApiResult<string> Login([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            if (login == null || password == null)
                new ApiResult<string>(new Error { Message = "Отсутсвует один из параметров запроса (Internal error)." });
            var user = _userRepository.GetUser(login);
            if (user == null)
                new ApiResult<string>(new Error { Message = "Логин не найден в системе." });
            var credentials = user.Password.Equals(password);
            if (!credentials)
                new ApiResult<string>(new Error { Message = "Ошибка авторизации. Неверный пароль." });
            return new ApiResult<string>(JwtHelper.GenerateToken(user.UserId));
        }

        [HttpPost("Registration")]
        public ApiResult<string> Registration([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            var passwordConfirmation = (string)jsonBody["passwordConfirm"];
            var email = (string)jsonBody["email"];
            if (login == null || password == null || passwordConfirmation == null || email == null)
                return new ApiResult<string>(new Error { Message = "Отсутсвует один из параметров запроса (Internal error)." });
            if (!password.Equals(passwordConfirmation))
                return new ApiResult<string>(new Error { Message = "Пароли не совпадают." });
            if (_userRepository.GetUserByEMail(email) != null)
                return new ApiResult<string>(new Error { Message = "Пользователь с указанным адресом электронной почты уже существует." });
            if (_userRepository.GetUser(login) != null)
                return new ApiResult<string>(new Error { Message = "Указанный логин уже существует, попробуйте использовать другой." });
            //TODO: add user to db
            return new ApiResult<string>("Ok");
        }

        #region Access Recovery

        [HttpPost("SendPageAccess")]
        public ApiResult<string> SendMailWithPageAccess([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var pageAccessAddress = (string)jsonBody["pageAccess"];
            if (login == null || pageAccessAddress == null)
                return new ApiResult<string>(new Error { Message = "Отсутсвует один из параметров запроса (Internal error)." });
            var user = _userRepository.GetUser(login);
            if (user == null)
                return new ApiResult<string>(new Error { Message = "Пользователь отсутствует в системе." });
            try
            {
                SendEmailAsync(user.Email, pageAccessAddress).GetAwaiter();
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(new Error { Message = exception.Message });
            }
            return new ApiResult<string>("Ok");
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

        [HttpPost("PasswordRecovery")]
        public ApiResult<string> RecoveryPassword([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var newPassword = (string)jsonBody["newPassword"];
            if (login == null || newPassword == null)
                return new ApiResult<string>(new Error { Message = "Отсутсвует один из параметров запроса (Internal error)." });
            var user = _userRepository.GetUser(login);
            if (user == null)
                return new ApiResult<string>(new Error { Message = "Пользователь отсутствует в системе." });
            //TODO: update user password in db 
            //_userRepository.Entry(user).State = EntityState.Modified;
            //user.Password = newPassword;
            //_userRepository.SaveChanges();
            return new ApiResult<string>("Ok");
        }

        #endregion
    }
}
