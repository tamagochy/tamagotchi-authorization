using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RegistrationModel
    {
        [Required, JsonProperty("login")]
        public string Login { get; set; }
        [Required, JsonProperty("password")]
        public string Password { get; set; }
        [Required, JsonProperty("passwordConfirm")]
        public string PasswordConfirm { get; set; }
        [Required, JsonProperty("email")]
        public string Email { get; set; }

    }
}
