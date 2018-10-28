using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RecoveryPasswordModel
    {
        [Required, JsonProperty("login")]
        [MinLength(3), MaxLength(24)]
        public string Login { get; set; }
        [Required, JsonProperty("newPassword")]
        [MinLength(8), MaxLength(24)]
        public string NewPassword { get; set; }
    }
}
