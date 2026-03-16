namespace SU26_PRN222_Healthcare.Models
{
    public class AppointmentReportViewModel
    {
        public AppointmentFilterViewModel Filter { get; set; } = new();

        // DỮ LIỆU ĐỔ RA TABLE
        public List<AppointmentListItemViewModel> Items { get; set; } = new();

        // SUMMARY
        public int TotalAppointments { get; set; }
        public int TotalCancelled { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }

        // SUM DEMO:
        // ví dụ tổng MaxPatients của các bác sĩ xuất hiện trong tập kết quả
        public int TotalDoctorMaxPatients { get; set; }
    }
}
