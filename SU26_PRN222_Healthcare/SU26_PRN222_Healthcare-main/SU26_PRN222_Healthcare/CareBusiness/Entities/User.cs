using System;
using System.ComponentModel.DataAnnotations;

namespace SU26_PRN222_Healthcare.CareBusiness.Entities
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Patient"; // "Admin" | "Patient"

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
