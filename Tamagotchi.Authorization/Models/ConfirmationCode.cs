using NpgsqlTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Authorization.Models
{
    public class ConfirmationCode
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CodeValue { get; set; }
        public DateTime CreationTime { get; set; }
        public bool Active { get; set; }
    }
}
