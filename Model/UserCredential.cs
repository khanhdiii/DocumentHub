using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentHub.Model
{
    public class UserCredential
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string PIN { get; set; } = "";

        [Required]
        public string SecurityQuestion1 { get; set; } = "";

        [Required]
        public string SecurityAnswer1 { get; set; } = "";

        [Required]
        public string SecurityQuestion2 { get; set; } = "";

        [Required]
        public string SecurityAnswer2 { get; set; } = "";

        [Required]
        public string SecondaryPassword { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastUpdated { get; set; }
    }
}
