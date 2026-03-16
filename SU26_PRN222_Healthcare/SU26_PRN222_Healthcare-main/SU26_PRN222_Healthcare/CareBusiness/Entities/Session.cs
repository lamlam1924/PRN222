using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SU26_PRN222_Healthcare.CareBusiness.Entities
{
    public class Session
    {
        [Key]
        [MaxLength(100)]
        public string SessionID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int UserID { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        // Navigation
        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}
