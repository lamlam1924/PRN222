using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.Models;

namespace SU26_PRN222_Healthcare.CareRepositories
{
    public interface IAppointmentRepository
    {
        // Booking
        void Add(Appointment appointment);
        int CountByDoctorDate(int doctorId, DateTime date);
        bool HasPatientBooked(int patientId, int doctorId, DateTime date);

        // Patient: view own appointments (all, include cancelled)
        IEnumerable<Appointment> GetByPatient(int patientId);

        // Cancellation
        Appointment? GetById(int id);
        void Cancel(int id);

        // Admin: view all with filters
        IEnumerable<Appointment> GetAll(DateTime? date = null, int? doctorId = null, int? patientId = null);

        // Upcoming & Past for a patient
        IEnumerable<Appointment> GetUpcomingByPatient(int patientId);
        IEnumerable<Appointment> GetPastByPatient(int patientId);

        // Stats
        int CountTotal();
        int CountCancelled();
        int CountActive();
        int CountToday();
        IEnumerable<(int DoctorId, string DoctorName, string Specialty, int Total, int ActiveCount)> GetStatsByDoctor();
        IEnumerable<(string Specialty, int Count)> GetStatsBySpecialty();

        //demo
        Task<AppointmentReportViewModel> GetReportAsync(AppointmentFilterViewModel filter);
    }
}
