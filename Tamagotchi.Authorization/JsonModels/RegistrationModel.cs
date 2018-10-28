using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RegistrationModel
    {
        [Required, JsonProperty("login")]
        [Range(3, 24)]
        public string Login { get; set; }
        [Required, JsonProperty("password")]
        [Range(8, 24)]
        public string Password { get; set; }
        [Required, JsonProperty("passwordConfirm")]
        [Range(8, 24)]
        public string PasswordConfirm { get; set; }
        [Required, JsonProperty("email")]
        public string Email { get; set; }

    }
}
