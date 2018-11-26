using Newtonsoft.Json;

namespace Tamagotchi.Authorization.JsonModels
{
    public class UserModel
    {
        //[JsonProperty("user_id")]
        public int UserId { get; set; }

        //[JsonProperty("user_login")]
        public string Login { get; set; }
    }
}
