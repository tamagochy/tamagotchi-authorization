using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class SendingMailModel
    {
        [Required, JsonProperty("login")]
        [MinLength(3), MaxLength(24)]
        public string Login { get; set; }
        [Required, JsonProperty("pageAccess")]
        public string PageAccess { get; set; }
    }
}
