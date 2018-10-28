using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class LoginModel
    {
        [Required, JsonProperty("login")]
        [MinLength(3), MaxLength(24)]
        public string Login { get; set; }
        [Required, JsonProperty("password")]
        [MinLength(8), MaxLength(24)]
        public string Password { get; set; }
    }
}
