﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class LoginModel
    {
        [Required, JsonProperty("login")]
        [Range(3,24)]
        public string Login { get; set; }
        [Required, JsonProperty("password")]
        [Range(8,24)]
        public string Password { get; set; }
    }
}
