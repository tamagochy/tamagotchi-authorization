using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Core;
using Tamagotchi.Authorization.Helpers;
using Tamagotchi.Authorization.JsonModels;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Controllers
{
    [Route("/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly AppInfo _appInfo;
        public AuthController(IUserRepository userRepository, IOptions<AppInfo> appInfo)
        {
            _userRepository = userRepository;
            _appInfo = appInfo.Value;
        }

        [HttpGet("getversion")]
        public string GetVersion()
        {
            dynamic jsonObject = new JObject();
            jsonObject.version = _appInfo.ProjectVersion;
            return jsonObject.ToString();
        }
        
        [HttpPost("login")]
        public ApiResult<string> Login([FromBody] LoginModel loginModel)
        {
            if (loginModel.Login == null || loginModel.Password == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(loginModel.Login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка чтения БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Логин не найден в системе.",
                        Code = "business.Error"
                    });
            var credentials = user.Password.Equals(loginModel.Password);
            if (!credentials)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка авторизации. Неверный пароль.",
                        Code = "https://i.ytimg.com/vi/qkudeorV03o/maxresdefault.jpg"
                    });
            return new ApiResult<string>(JwtHelper.GenerateToken(user.UserId, _appInfo.SecretKey));
        }

        [HttpPost("registration")]
        public ApiResult<string> Registration([FromBody] RegistrationModel registrationModel)
        {
            if (registrationModel.Login == null || registrationModel.Password == null || registrationModel.PasswordConfirm == null || 
                registrationModel.Email == null || registrationModel.Pet == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            if (!registrationModel.Password.Equals(registrationModel.PasswordConfirm))
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Пароли не совпадают.",
                        Code = "business.Error"
                    });
            if (_userRepository.GetUserByEmail(registrationModel.Email) != null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Пользователь с указанным адресом электронной почты уже существует.",
                        Code = "business.Error"
                    });
            if (_userRepository.GetUserByLogin(registrationModel.Login) != null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Указанный логин уже существует, попробуйте использовать другой.",
                        Code = "business.Error"
                    });
            try
            {
                _userRepository.AddUser(
                    new User
                    {
                        Login = registrationModel.Login,
                        Password = registrationModel.Password,
                        Email = registrationModel.Email,
                        Pet = int.Parse(registrationModel.Pet)
                    });
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка записи в БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            return new ApiResult<string>("Ok");
        }

        #region Access Recovery

        [HttpPost("sendpageaccess")]
        public ApiResult<string> SendMailWithPageAccess([FromBody] SendingMailModel sendingMailModel)
        {
            if (sendingMailModel.Login == null || sendingMailModel.PageAccess == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(sendingMailModel.Login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка чтения БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Пользователь отсутствует в системе.",
                        Code = "business.Error"
                    });
            try
            {
                SendEmailAsync(user.Email, sendingMailModel.PageAccess, _appInfo.ApplicationEmail, _appInfo.EmailPassword).GetAwaiter();
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = exception.Message,
                        Code = "server.Error"
                    });
            }
            return new ApiResult<string>("Ok");
        }

        private static async Task SendEmailAsync(string userMail, string pageAccess, string appEmail, string appPassword)
        {
            var from = new MailAddress(appEmail);
            var to = new MailAddress(userMail);
            var message = new MailMessage(from, to)
            {
                Subject = "Восстановление доступа",
                Body = "Вы хотите восстановить пароль доступа.\nПожалуйста, посетите страницу восстановления:\n" + pageAccess
            };
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(appEmail, appPassword),
                EnableSsl = true
            };
            await smtp.SendMailAsync(message);
        }

        [HttpPost("passwordrecovery")]
        public ApiResult<string> RecoveryPassword([FromBody] RecoveryPasswordModel recoveryPasswordModel)
        {
            if (recoveryPasswordModel.Login == null || recoveryPasswordModel.NewPassword == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(recoveryPasswordModel.Login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка чтения из БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Пользователь отсутствует в системе.",
                        Code = "business.Error"
                    });
            try
            {
                _userRepository.UpdatePassword(user, recoveryPasswordModel.NewPassword);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Attr = "Ошибка записи в БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            return new ApiResult<string>("Ok");
        }

        #endregion
    }
}
