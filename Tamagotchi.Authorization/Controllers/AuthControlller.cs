using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Core;
using Tamagotchi.Authorization.Helpers;
using Tamagotchi.Authorization.JsonModels;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly AppInfo _appInfo;
        public AuthController(IUserRepository userRepository, IOptions<AppInfo> appInfo)
        {
            _userRepository = userRepository;
            _appInfo = appInfo.Value;
        }

        [HttpGet("version")]
        public VersionModel GetVersion() =>
           new VersionModel { Version = _appInfo.ProjectVersion };

        [HttpPost("login")]
        public ApiResult<string> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                    }
                );
            }

            User user;
            try
            {
                user = _userRepository.GetUserByLogin(loginModel.Login);
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "server.Error"
                        }
                    }
                );
            }
            if (user == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Attr = "Логин не найден в системе.",
                            Code = "business.Error"
                        }
                    }
                );
            }
            var credentials = Hashing.ValidatePassword(loginModel.Password, user.Password);
            if (!credentials)
            {
                HttpContext.Response.StatusCode = 401;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "https://i.ytimg.com/vi/qkudeorV03o/maxresdefault.jpg"
                        }
                    }
                );
            }
            return new ApiResult<string>(JwtHelper.GenerateToken(user.UserId, _appInfo.SecretKey));
        }

        [HttpPost("register")]
        public ApiResult<string> Registration([FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                    }
                );
            }
            if ((!registrationModel.Password.Equals(registrationModel.PasswordConfirm))
                || (_userRepository.GetUserByEmail(registrationModel.Email) != null) ||
                    (_userRepository.GetUserByLogin(registrationModel.Login) != null))
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                new List<Error>
                {
                    new Error
                     {
                         Code = "business.Error"
                     }
                });
            }
            try
            {
                _userRepository.AddUser(
                    new User
                    {
                        Login = registrationModel.Login,
                        Password = registrationModel.Password,
                        Email = registrationModel.Email
                    });
            }
            catch 
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                new List<Error>
                {
                    new Error
                    {
                        Code = "server.Error"
                    }
                });
            }
            return new ApiResult<string>("Ok");
        }

        #region Access Recovery

        [HttpPost("password/recover")]
        public ApiResult<string> SendMailWithPageAccess([FromBody] SendingMailModel sendingMailModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Keys.ToList();
                HttpContext.Response.StatusCode = 400;
                var apiResult = new ApiResult<string> { Errors = new List<Error>() };

                foreach (var err in errors)
                {
                    apiResult.Errors.Add(new Error
                    {
                        Attr = err,
                        Code = "validation.Incorrect"
                    });
                }
                return apiResult;
            }
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(sendingMailModel.Login);
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "server.Error"
                        }
                    });
            }
            if (user == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.Error"
                        }
                    });
            }
            try
            {
                SendEmailAsync(user.Email, sendingMailModel.PageAccess, _appInfo.ApplicationEmail, _appInfo.EmailPassword).GetAwaiter();
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                   new List<Error>
                   {
                       new Error
                       {
                           Code = "server.Error"
                       }
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
                Body = "Вы хотите восстановить пароль доступа. Пожалуйста, посетите страницу восстановления: " + pageAccess
            };
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(appEmail, appPassword),
                EnableSsl = true
            };
            await smtp.SendMailAsync(message);
        }

        [HttpPost("password/recover/confirm")]
        public ApiResult<string> RecoveryPassword([FromBody] RecoveryPasswordModel recoveryPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                   new List<Error>
                   {
                       new Error
                       {
                           Code = "protocol.Incorrect"
                       }
                   });
            }
            User user;
            try
            {
                user = _userRepository.GetUserByLogin(recoveryPasswordModel.Login);
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                   new List<Error>
                   {
                       new Error
                       {
                           Code = "server.Error"
                       }
                   });
            }
            if (user == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                   new List<Error>
                   {
                       new Error
                       {
                            Code = "business.Error"
                       }
                   });
            }
            try
            {
                _userRepository.UpdatePassword(user, recoveryPasswordModel.NewPassword);
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "server.Error"
                        }
                    });
            }
            return new ApiResult<string>("Ok");
        }

        #endregion
    }
}
