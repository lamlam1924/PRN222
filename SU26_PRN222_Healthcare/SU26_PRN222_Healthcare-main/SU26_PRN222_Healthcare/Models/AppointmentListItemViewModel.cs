namespace SU26_PRN222_Healthcare.Models
{
    public class AppointmentListItemViewModel
    {
        public int AppointmentID { get; set; }

        public int PatientID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;

        public int DoctorID { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }
        public DateTime AppointmentDate { get; set; }

        public bool IsCancelled { get; set; }
        public bool DoctorActive { get; set; }
        public int DoctorMaxPatients { get; set; }
    }
}
