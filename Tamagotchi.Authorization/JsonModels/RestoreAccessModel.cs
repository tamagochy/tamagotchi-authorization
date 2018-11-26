using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RestoreAccessModel
    {
        [Required, JsonProperty("confirmationCode")]
        [MinLength(60), MaxLength(60)]
        public string ConfirmationCode { get; set; }

        [Required, JsonProperty("newPassword")]
        [MinLength(8), MaxLength(24)]
        public string NewPassword { get; set; }
        [Required, JsonProperty("repeatedNewPassword")]
        [MinLength(8), MaxLength(24)]
        public string RepeatedNewPassword { get; set; }
    }

}
