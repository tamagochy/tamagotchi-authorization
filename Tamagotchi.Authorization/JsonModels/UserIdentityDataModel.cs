using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class UserIdentityDataModel
    {
        [Required, JsonProperty("userIdentityData")]
        public string UserIdentityData { get; set; }
    }
}
