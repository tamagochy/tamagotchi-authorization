using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class SendingMailModel
    {
        [Required, JsonProperty("login")]
        [Range(3, 24)]
        public string Login { get; set; }
        [Required, JsonProperty("pageAccess")]
        [Range(8, 24)]
        public string PageAccess { get; set; }
    }
}
