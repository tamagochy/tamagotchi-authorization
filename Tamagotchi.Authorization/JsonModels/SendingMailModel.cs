using Newtonsoft.Json;

namespace Tamagotchi.Authorization.JsonModels
{
    public class SendingMailModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("pageAccess")]
        public string PageAccess { get; set; }
    }
}
