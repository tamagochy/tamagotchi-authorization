﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Core;
using Tamagotchi.Authorization.Helpers;
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

        [HttpPost("login")]
        public ApiResult<string> Login([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            if (login == null || password == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка чтения БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Логин не найден в системе.",
                        Code = "business.Error"
                    });
            var credentials = user.Password.Equals(password);
            if (!credentials)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка авторизации. Неверный пароль.",
                        Code = "https://i.ytimg.com/vi/qkudeorV03o/maxresdefault.jpg"
                    });
            return new ApiResult<string>(JwtHelper.GenerateToken(user.UserId, _appInfo.SecretKey));
        }

        [HttpPost("registration")]
        public ApiResult<string> Registration([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var password = (string)jsonBody["password"];
            var passwordConfirmation = (string)jsonBody["passwordConfirm"];
            var email = (string)jsonBody["email"];
            var pet = (string)jsonBody["pet"];
            if (login == null || password == null || passwordConfirmation == null || email == null || pet == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            if (!password.Equals(passwordConfirmation))
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Пароли не совпадают.",
                        Code = "business.Error"
                    });
            if (_userRepository.GetUserByEmail(email) != null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Пользователь с указанным адресом электронной почты уже существует.",
                        Code = "business.Error"
                    });
            if (_userRepository.GetUserByLogin(login) != null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Указанный логин уже существует, попробуйте использовать другой.",
                        Code = "business.Error"
                    });
            try
            {
                _userRepository.AddUser(
                    new User
                    {
                        Login = login,
                        Password = password,
                        Email = email,
                        Pet = int.Parse(pet)
                    });
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка записи в БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            return new ApiResult<string>("Ok");
        }

        #region Access Recovery

        [HttpPost("sendpageaccess")]
        public ApiResult<string> SendMailWithPageAccess([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var pageAccessAddress = (string)jsonBody["pageAccess"];
            if (login == null || pageAccessAddress == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка чтения БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Пользователь отсутствует в системе.",
                        Code = "business.Error"
                    });
            try
            {
                SendEmailAsync(user.Email, pageAccessAddress, _appInfo.ApplicationEmail, _appInfo.EmailPassword).GetAwaiter();
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = exception.Message,
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
        public ApiResult<string> RecoveryPassword([FromBody] JObject jsonBody)
        {
            var login = (string)jsonBody["login"];
            var newPassword = (string)jsonBody["newPassword"];
            if (login == null || newPassword == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Отсутсвует один из параметров запроса.",
                        Code = "protocol.Incorrect"
                    });
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(login);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка чтения из БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            if (user == null)
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Пользователь отсутствует в системе.",
                        Code = "business.Error"
                    });
            try
            {
                _userRepository.UpdatePassword(user, newPassword);
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(
                    new Error
                    {
                        Message = "Ошибка записи в БД. " + exception.Message,
                        Code = "server.Error"
                    });
            }
            return new ApiResult<string>("Ok");
        }

        #endregion
    }
}