using System.ComponentModel.DataAnnotations;

namespace SU26_PRN222_Healthcare.CareBusiness.Entities
{
    public class Doctor
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string DoctorName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int MaxPatients { get; set; } = 10;

        public bool Active { get; set; } = true;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
