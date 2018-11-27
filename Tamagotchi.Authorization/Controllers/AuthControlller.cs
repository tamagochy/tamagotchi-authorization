using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class AuthController : AuthControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmationCodeRepository _codeRepository;
        private readonly AppInfo _appInfo;
        public AuthController(IUserRepository userRepository, IConfirmationCodeRepository codeRepository, IOptions<AppInfo> appInfo)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _appInfo = appInfo.Value;
        }

        [HttpGet("version")]
        public VersionModel GetVersion() =>
           new VersionModel { Version = _appInfo.ProjectVersion };

        #region Login

        [HttpPost("login")]
        public async Task<ApiResult<string>> Login([FromBody] JObject loginModel)
        {
            if (loginModel == null)
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
            var validationResult = ValidateLoginModel(loginModel);
            if (validationResult.Item2.Any())
            {
                HttpContext.Response.StatusCode = 400;
                var apiResult = new ApiResult<string> { Errors = new List<Error>() };
                foreach (var error in validationResult.Item2)
                {
                    apiResult.Errors.Add(new Error
                    {
                        Attr = error.Attribute,
                        Code = error.Error
                    });
                }
                return apiResult;
            }
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
                user = await _userRepository.GetUserByLogin(validationResult.Item1.Login);
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.UserNotFound"
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
                            Code = "business.UserNotFound"
                        }
                    }
                );
            }
            if (!Hashing.ValidatePassword(validationResult.Item1.Password, user.Password))
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<string>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.BadCredentials"
                        }
                    }
                );
            }
            return new ApiResult<string>(JwtHelper.GenerateToken(user.UserId, _appInfo.SecretKey, _appInfo.LifeTimeToken));
        }

        [HttpPost("getUserLogins")]
        public async Task<ApiResult<dynamic>> GetUserLogins([FromBody] JArray idsArray)
        {
            var validationResult = ValidateUserIds(idsArray);
            if (idsArray == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                    }
                );
            }
            try
            {
                if (validationResult.Item2.Any())
                {
                    HttpContext.Response.StatusCode = 400;
                    var apiResult = new ApiResult<string> { Errors = new List<Error>() };
                    foreach (var error in validationResult.Item2)
                    {
                        apiResult.Errors.Add(new Error
                        {
                            Attr = error.Attribute,
                            Code = error.Error
                        });
                    }
                    return new ApiResult<dynamic>(apiResult);
                }
                if (!ModelState.IsValid)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "protocol.Incorrect"
                            }
                        }
                    );
                }
                var users = await _userRepository.GetUsersByIds(validationResult.Item1);
                var result = new List<UserModel>(users.Count());
                foreach (var us in users)
                {
                    result.Add(new UserModel
                    {
                        UserId = us.UserId,
                        Login = us.Login
                    });
                }
                return new ApiResult<dynamic>(result);
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                var apiResult = new ApiResult<string> { Errors = new List<Error>() };
                foreach (var error in validationResult.Item2)
                {
                    apiResult.Errors.Add(new Error
                    {
                        Attr = error.Attribute,
                        Code = error.Error
                    });
                }
                return new ApiResult<dynamic>(apiResult);
            }
        }

        #endregion

        #region Registration

        [HttpPost("registration")]
        public async Task<ApiResult<dynamic>> Registration([FromBody] JObject registrationModel)
        {
            try
            {
                if (registrationModel == null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                        }
                    );
                }
                var validationResult = ValidateRegistrationModel(registrationModel);
                if (validationResult.Item2.Any())
                {
                    HttpContext.Response.StatusCode = 400;
                    var apiResult = new ApiResult<dynamic> { Errors = new List<Error>() };
                    foreach (var error in validationResult.Item2)
                    {
                        apiResult.Errors.Add(new Error
                        {
                            Attr = error.Attribute,
                            Code = error.Error
                        });
                    }
                    return apiResult;
                }
                if (!ModelState.IsValid)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                        }
                    );
                }
                if (!validationResult.Item1.Password.Equals(validationResult.Item1.PasswordConfirm))
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                    new List<Error>
                    {
                    new Error
                    {
                        Code = "business.PasswordsNotEquals"
                    }
                    });
                }
                if (await _userRepository.CheckExistsUser(validationResult.Item1.Login.ToLower(), validationResult.Item1.Email.ToLower()))
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                             Code = "business.UserAlreadyExists"
                        }
                    });
                }
                await _userRepository.AddUser(
                    new User
                    {
                        Login = validationResult.Item1.Login,
                        Password = validationResult.Item1.Password,
                        Email = validationResult.Item1.Email
                    }, _appInfo.CountRound);
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.UserNotFound"
                        }
                    });
            }
            return new ApiResult<dynamic>(new { succeed = true });
        }

        #endregion

        #region Access Recovery

        [HttpPost("restoreAccess")]
        public async Task<ApiResult<dynamic>> RestoreAccess([FromBody] JObject sendingMailModel)
        {
            if (sendingMailModel == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                    }
                );
            }
            var validationResult = ValidateUserIdentityData(sendingMailModel);
            if (validationResult.Item2.Any())
            {
                HttpContext.Response.StatusCode = 400;
                var apiResult = new ApiResult<dynamic> { Errors = new List<Error>() };
                foreach (var error in validationResult.Item2)
                {
                    apiResult.Errors.Add(new Error
                    {
                        Attr = error.Attribute,
                        Code = error.Error
                    });
                }
                return apiResult;
            }
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
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
                user = await _userRepository.GetUserByLogin(validationResult.Item1.UserIdentityData);
                if (user == null)
                    user = await _userRepository.GetUserByEmail(validationResult.Item1.UserIdentityData);
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.UserNotFound"
                        }
                    });
            }
            if (user == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.UserNotFound"
                        }
                    });
            }
            var confirmationCode = Hashing.HashPassword((user.UserId + DateTime.UtcNow.Millisecond).ToString(), _appInfo.CountRound);
            try
            {
                await _codeRepository.AddConfirmationCode(user.UserId, confirmationCode);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "server.Error" + ex.Message
                        }
                    });
            }
            await SendEmailAsync(user.Email, user.Login, _appInfo.ApplicationUrl + "/restoreAccessConfirm/?confirmationCode=" + confirmationCode,
                _appInfo.ApplicationEmail, _appInfo.EmailPassword);
            return new ApiResult<dynamic>(new { succeed = true });
        }

        private static async Task SendEmailAsync(string userMail, string login, string pageAccess, string appEmail, string appPassword)
        {
            var from = new MailAddress(appEmail);
            var to = new MailAddress(userMail);
            var message = new MailMessage(from, to)
            {
                Subject = "Восстановление доступа",
                Body = login + ", Вы запросили процедуру восстановления пароля в приложении \"Тамагочи\". " +
                    "Для продолжения восстановления пароля перейдите по ссылке: " + pageAccess +
                    ". Если Вы не запрашивали процедуру восстановления пароля, пожалуйста, игнорируйте это письмо."
            };
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(appEmail, appPassword),
                EnableSsl = true
            };
            await smtp.SendMailAsync(message);
        }

        [HttpPost("restoreAccessConfirm")]
        public async Task<ApiResult<dynamic>> RestoreAccessConfirm([FromBody] JObject recoveryPasswordModel)
        {
            var validationResult = ValidateRestoreAccessModel(recoveryPasswordModel);
            if (recoveryPasswordModel == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "protocol.Incorrect"
                        }
                    }
                );
            }
            try
            {
                if (validationResult.Item2.Any())
                {
                    HttpContext.Response.StatusCode = 400;
                    var apiResult = new ApiResult<dynamic> { Errors = new List<Error>() };
                    foreach (var error in validationResult.Item2)
                    {
                        apiResult.Errors.Add(new Error
                        {
                            Attr = error.Attribute,
                            Code = error.Error
                        });
                    }
                    return apiResult;
                }
                if (!ModelState.IsValid)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "protocol.Incorrect"
                            }
                        }
                    );
                }
                if (!validationResult.Item1.NewPassword.Equals(validationResult.Item1.RepeatedNewPassword))
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                    new List<Error>
                    {
                        new Error
                        {
                            Code = "business.PasswordsNotEquals"
                        }
                    });
                }
                var confirmation = await _codeRepository.GetConfirmData(validationResult.Item1.ConfirmationCode);
                if (confirmation == null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "business.ConfirmCodeNotFound"
                            }
                        });
                }
                if (!confirmation.Active)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "business.Inactive"
                            }
                        });
                }
                var dateTimeNow = DateTime.UtcNow.Ticks;
                var creationTime = confirmation.CreationTime.Ticks;
                if (!(dateTimeNow >= creationTime && dateTimeNow <=
                    creationTime + TimeSpan.FromMinutes(_appInfo.ConfirmCodeLifeTime).Ticks))
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "business.Expired"
                            }
                        });
                }
                var user = await _userRepository.GetUserById(confirmation.UserId);
                if (user == null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<dynamic>(
                        new List<Error>
                        {
                            new Error
                            {
                                Code = "business.UserNotFound"
                            }
                        }
                    );
                }
                await _userRepository.UpdatePassword(user, validationResult.Item1.NewPassword, _appInfo.CountRound);
                await _codeRepository.SetConfirmCodeNotActive(confirmation);
                return new ApiResult<dynamic>(new { succeed = true });
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<dynamic>(
                    new List<Error>
                    {
                            new Error
                            {
                                Code = "business.UserNotFound"
                            }
                    }
                );
            }
        }

        #endregion


    }
}


