using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class LoginModel
    {
        [Required, JsonProperty("login")]
        public string Login { get; set; }
        [Required, JsonProperty("password")]
        public string Password { get; set; }
    }
}
