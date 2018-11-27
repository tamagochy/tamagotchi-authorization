using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.JsonModels
{
    public class RestoreAccessModel
    {
        [Required, JsonProperty("confirmationCode")]
        public string ConfirmationCode { get; set; }

        [Required, JsonProperty("newPassword")]
        public string NewPassword { get; set; }
        [Required, JsonProperty("repeatedNewPassword")]
        public string RepeatedNewPassword { get; set; }
    }

}
