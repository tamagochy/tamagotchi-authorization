using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Pet { get; set; }
    }
}
