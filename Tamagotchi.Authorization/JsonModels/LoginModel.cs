using Newtonsoft.Json;

namespace Tamagotchi.Authorization.JsonModels
{
    public class LoginModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
