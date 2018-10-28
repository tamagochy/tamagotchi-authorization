using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class VersionModel
    {
        [Required, JsonProperty("version")]
        public string Version { get; set; }
    }
}
