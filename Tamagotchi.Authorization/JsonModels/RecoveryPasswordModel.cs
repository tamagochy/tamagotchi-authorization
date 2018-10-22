using Newtonsoft.Json;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RecoveryPasswordModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
