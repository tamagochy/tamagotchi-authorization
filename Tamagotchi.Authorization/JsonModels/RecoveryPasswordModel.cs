using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RecoveryPasswordModel
    {
        [Required, JsonProperty("login")]
        [Range(3, 24)]
        public string Login { get; set; }
        [Required, JsonProperty("newPassword")]
        [Range(8, 24)]
        public string NewPassword { get; set; }
    }
}
