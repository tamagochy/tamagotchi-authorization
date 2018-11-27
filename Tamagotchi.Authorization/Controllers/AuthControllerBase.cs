using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tamagotchi.Authorization.BusinessModel;
using Tamagotchi.Authorization.JsonModels;

namespace Tamagotchi.Authorization.Controllers
{
    public class AuthControllerBase : Controller
    {
        private readonly Regex EmailParser =
          new Regex("^[A-Za-z]\\S+@[A-Za-z]+\\.[A-Za-z]{2,5}$");


        public Tuple<LoginModel, List<ErrorDescription>> ValidateLoginModel(JObject requestParams)
        {
            var loginModel = new LoginModel();
            var errors = new List<ErrorDescription>();
            loginModel.Login = (string)requestParams["login"];
            loginModel.Password = (string)requestParams["password"];
            if (loginModel.Login == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "login" });
            else if (loginModel.Login.Length < 3 || loginModel.Login.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "login" });
            if (loginModel.Password == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "password" });
            else if (loginModel.Password.Length < 8 || loginModel.Password.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "password" });
            return new Tuple<LoginModel, List<ErrorDescription>>(loginModel, errors);
        }

        public Tuple<RegistrationModel, List<ErrorDescription>> ValidateRegistrationModel(JObject requestParams)
        {
            var registrationModel = new RegistrationModel();
            var errors = new List<ErrorDescription>();
            registrationModel.Login = (string)requestParams["login"];
            registrationModel.Password = (string)requestParams["password"];
            registrationModel.PasswordConfirm = (string)requestParams["passwordConfirm"];
            registrationModel.Email = (string)requestParams["email"];
            if (registrationModel.Login == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "login" });
            if (registrationModel.Login.Length < 3 || registrationModel.Login.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "login" });
            if (registrationModel.Password == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "password" });
            if (registrationModel.Password.Length < 8 || registrationModel.Password.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "password" });
            if (registrationModel.PasswordConfirm == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "passwordConfirm" });
            else if (registrationModel.PasswordConfirm.Length < 8 || registrationModel.PasswordConfirm.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "passwordConfirm" });
            if (registrationModel.Email == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "email" });
            else if (registrationModel.Email.Length > 100 || !EmailParser.IsMatch(registrationModel.Email))
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "email" });
            return new Tuple<RegistrationModel, List<ErrorDescription>>(registrationModel, errors);
        }

        public Tuple<UserIdentityDataModel, List<ErrorDescription>> ValidateUserIdentityData(JObject requestParams)
        {
            var recoveryPasswordModel = new UserIdentityDataModel();
            var errors = new List<ErrorDescription>();
            recoveryPasswordModel.UserIdentityData = (string)requestParams["userIdentityData"];
            if (recoveryPasswordModel.UserIdentityData == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "userIdentityData" });
            return new Tuple<UserIdentityDataModel, List<ErrorDescription>>(recoveryPasswordModel, errors);
        }

        public Tuple<RestoreAccessModel, List<ErrorDescription>> ValidateRestoreAccessModel(JObject requestParams)
        {
            var recoveryPasswordModel = new RestoreAccessModel();
            var errors = new List<ErrorDescription>();
            recoveryPasswordModel.ConfirmationCode = (string)requestParams["confirmationCode"];
            recoveryPasswordModel.NewPassword = (string)requestParams["newPassword"];
            recoveryPasswordModel.RepeatedNewPassword = (string)requestParams["repeatedNewPassword"];
            if (recoveryPasswordModel.ConfirmationCode == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "confirmationCode" });
            else if (recoveryPasswordModel.ConfirmationCode.Length != 60)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "confirmationCode" });
            if (recoveryPasswordModel.NewPassword == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "newPassword" });
            else if (recoveryPasswordModel.NewPassword.Length < 8 || recoveryPasswordModel.NewPassword.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "newPassword" });
            if (recoveryPasswordModel.RepeatedNewPassword == null)
                errors.Add(new ErrorDescription { Error = "validation.Missing", Attribute = "repeatedNewPassword" });
            else if (recoveryPasswordModel.RepeatedNewPassword.Length < 8 || recoveryPasswordModel.RepeatedNewPassword.Length > 24)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "repeatedNewPassword" });
            return new Tuple<RestoreAccessModel, List<ErrorDescription>>(recoveryPasswordModel, errors);
        }

        public Tuple<int[], List<ErrorDescription>> ValidateUserIds(JArray requestParams)
        {
            var errors = new List<ErrorDescription>();
            var result = new int[requestParams.Count];
            result = requestParams.ToObject<int[]>();
            if (result == null || requestParams.Count < 1)
                errors.Add(new ErrorDescription { Error = "validation.Incorrect", Attribute = "request.body" });
            return new Tuple<int[], List<ErrorDescription>>(result, errors);
        }


    }
}
