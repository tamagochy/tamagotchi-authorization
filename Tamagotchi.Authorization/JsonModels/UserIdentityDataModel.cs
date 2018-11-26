using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class UserIdentityDataModel
    {
        [Required, JsonProperty("userIdentityData")]
        [MinLength(3), MaxLength(24)]
        public string UserIdentityData { get; set; }
    }
}
