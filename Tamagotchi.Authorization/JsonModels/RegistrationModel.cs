using Newtonsoft.Json;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RegistrationModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("passwordConfirm")]
        public string PasswordConfirm { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("pet")]
        public string Pet { get; set; }

    }
}
