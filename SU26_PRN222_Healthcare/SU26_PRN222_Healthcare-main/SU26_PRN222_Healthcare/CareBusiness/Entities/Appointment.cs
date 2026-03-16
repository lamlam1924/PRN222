using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SU26_PRN222_Healthcare.CareBusiness.Entities
{
    public class Appointment
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime AppointmentDate { get; set; }

        public bool IsCancelled { get; set; } = false;

        // Navigation
        [ForeignKey(nameof(PatientID))]
        public User? Patient { get; set; }

        [ForeignKey(nameof(DoctorID))]
        public Doctor? Doctor { get; set; }
    }
}
