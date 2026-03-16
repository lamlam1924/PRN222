using SU26_PRN222_Healthcare.CareBusiness.Entities;

namespace SU26_PRN222_Healthcare.Models
{
    public class DoctorSlotViewModel
    {
        public Doctor Doctor { get; set; } = null!;
        public int Booked { get; set; }
        public int Remaining { get; set; }
        public int Percentage => Doctor.MaxPatients > 0
            ? (int)(Booked * 100.0 / Doctor.MaxPatients)
            : 0;
    }

    public class DashboardStatsViewModel
    {
        public int TotalDoctors { get; set; }
        public int ActiveDoctors { get; set; }
        public int InactiveDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int ActiveAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public List<(int DoctorId, string DoctorName, string Specialty, int Total, int ActiveCount)> StatsByDoctor { get; set; } = new();
        public List<(string Specialty, int Count)> StatsBySpecialty { get; set; } = new();
    }
}
