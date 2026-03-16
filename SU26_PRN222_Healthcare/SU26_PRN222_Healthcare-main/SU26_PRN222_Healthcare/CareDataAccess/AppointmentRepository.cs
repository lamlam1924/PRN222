using Microsoft.EntityFrameworkCore;
using SU26_PRN222_Healthcare.CareBusiness;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;
using SU26_PRN222_Healthcare.Models;

namespace SU26_PRN222_Healthcare.CareDataAccess
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly CareDbContext _context;

        public AppointmentRepository(CareDbContext context)
        {
            _context = context;
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public Appointment? GetById(int id)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.ID == id);
        }

        public void Cancel(int id)
        {
            var appt = _context.Appointments.Find(id);
            if (appt != null)
            {
                appt.IsCancelled = true;
                _context.SaveChanges();
            }
        }

        // Patient: all own appointments (including cancelled) joined with Doctor
        public IEnumerable<Appointment> GetByPatient(int patientId)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientID == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        // Admin: all appointments with optional filters, joined with Doctor + Patient
        public IEnumerable<Appointment> GetAll(DateTime? date = null, int? doctorId = null, int? patientId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorID == doctorId.Value);

            if (patientId.HasValue)
                query = query.Where(a => a.PatientID == patientId.Value);

            return query.OrderByDescending(a => a.AppointmentDate).ToList();
        }

        public IEnumerable<Appointment> GetUpcomingByPatient(int patientId)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientID == patientId
                         && !a.IsCancelled
                         && a.AppointmentDate.Date >= DateTime.Today)
                .OrderBy(a => a.AppointmentDate)
                .ToList();
        }

        public IEnumerable<Appointment> GetPastByPatient(int patientId)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientID == patientId
                         && a.AppointmentDate.Date < DateTime.Today)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public int CountByDoctorDate(int doctorId, DateTime date)
        {
            var targetDate = date.Date;
            return _context.Appointments
                .Count(a => a.DoctorID == doctorId
                         && !a.IsCancelled
                         && a.AppointmentDate.Date == targetDate);
        }

        public bool HasPatientBooked(int patientId, int doctorId, DateTime date)
        {
            var targetDate = date.Date;
            return _context.Appointments
                .Any(a => a.PatientID == patientId
                       && a.DoctorID == doctorId
                       && !a.IsCancelled
                       && a.AppointmentDate.Date == targetDate);
        }

        // Stats
        public int CountTotal() => _context.Appointments.Count();
        public int CountCancelled() => _context.Appointments.Count(a => a.IsCancelled);
        public int CountActive() => _context.Appointments.Count(a => !a.IsCancelled);
        public int CountToday()
        {
            var today = DateTime.Today;
            return _context.Appointments.Count(a => a.AppointmentDate.Date == today && !a.IsCancelled);
        }

        public IEnumerable<(int DoctorId, string DoctorName, string Specialty, int Total, int ActiveCount)> GetStatsByDoctor()
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .GroupBy(a => new { a.DoctorID, a.Doctor!.DoctorName, a.Doctor.Specialty })
                .Select(g => new
                {
                    g.Key.DoctorID,
                    g.Key.DoctorName,
                    g.Key.Specialty,
                    Total = g.Count(),
                    ActiveCount = g.Count(a => !a.IsCancelled)
                })
                .OrderByDescending(x => x.Total)
                .AsEnumerable()
                .Select(x => (x.DoctorID, x.DoctorName, x.Specialty, x.Total, x.ActiveCount))
                .ToList();
        }

        public IEnumerable<(string Specialty, int Count)> GetStatsBySpecialty()
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => !a.IsCancelled)
                .GroupBy(a => a.Doctor!.Specialty)
                .Select(g => new { Specialty = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .AsEnumerable()
                .Select(x => (x.Specialty, x.Count))
                .ToList();
        }

        public async Task<AppointmentReportViewModel> GetReportAsync(AppointmentFilterViewModel filter)
        {
            // =========================
            // NGUỒN DỮ LIỆU GỐC
            // =========================
            // CHỖ NÀY LẤY DỮ LIỆU TỪ DB
            // Có Include để lấy kèm Patient và Doctor
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            // =========================
            // FILTER: IncludeCancelled
            // =========================
            if (!filter.IncludeCancelled)
            {
                query = query.Where(x => !x.IsCancelled);
            }

            // =========================
            // FILTER: Keyword
            // =========================
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim().ToLower();

                query = query.Where(x =>
                    (x.Patient != null && x.Patient.FullName.ToLower().Contains(keyword)) ||
                    (x.Patient != null && x.Patient.Email.ToLower().Contains(keyword)) ||
                    (x.Doctor != null && x.Doctor.DoctorName.ToLower().Contains(keyword)) ||
                    (x.Doctor != null && x.Doctor.Specialty.ToLower().Contains(keyword))
                );
            }

            // =========================
            // FILTER: AppointmentDate / BookingDate
            // =========================
            if (filter.FromDate.HasValue)
            {
                var from = filter.FromDate.Value.Date;
                query = query.Where(x => x.AppointmentDate >= from);
            }

            if (filter.ToDate.HasValue)
            {
                var to = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.AppointmentDate <= to);
            }

            // =========================
            // FILTER: DoctorActive
            // =========================
            // nếu không cần có thể bỏ
            if (filter.DoctorActive.HasValue)
            {
                query = query.Where(x => x.Doctor != null && x.Doctor.Active == filter.DoctorActive.Value);
            }

            // =========================
            // FILTER: Specialty
            // =========================
            if (!string.IsNullOrWhiteSpace(filter.Specialty))
            {
                var specialty = filter.Specialty.Trim().ToLower();
                query = query.Where(x => x.Doctor != null && x.Doctor.Specialty.ToLower().Contains(specialty));
            }

            // =========================
            // SUMMARY TRƯỚC KHI TAKE TOP
            // =========================
            // NẾU MUỐN summary tính trên toàn bộ dữ liệu sau lọc
            // thì tính ở đây là hợp lý
            var totalAppointments = await query.CountAsync();
            var totalCancelled = await query.CountAsync(x => x.IsCancelled);
            var totalPatients = await query
                .Select(x => x.PatientID)
                .Distinct()
                .CountAsync();
            var totalDoctors = await query
                .Select(x => x.DoctorID)
                .Distinct()
                .CountAsync();

            // DEMO SUM:
            // Tổng MaxPatients của các doctor xuất hiện trong tập kết quả
            var totalDoctorMaxPatients = await query
                .Where(x => x.Doctor != null)
                .Select(x => new { x.DoctorID, x.Doctor!.MaxPatients })
                .Distinct()
                .SumAsync(x => (int?)x.MaxPatients) ?? 0;

            // =========================
            // SORT
            // =========================
            query = query
                .OrderByDescending(x => x.AppointmentDate)
                .ThenByDescending(x => x.BookingDate);

            // =========================
            // TOP
            // =========================
            // CHỖ NÀY GIỚI HẠN SỐ DÒNG HIỂN THỊ
            if (filter.Top.HasValue && filter.Top.Value > 0)
            {
                query = query.Take(filter.Top.Value);
            }

            // =========================
            // MAP DỮ LIỆU SANG VIEWMODEL
            // =========================
            // CHỖ NÀY LÀ CHỖ ĐỔ DỮ LIỆU RA VIEW
            var items = await query
                .Select(x => new AppointmentListItemViewModel
                {
                    AppointmentID = x.ID,

                    PatientID = x.PatientID,
                    PatientName = x.Patient != null ? x.Patient.FullName : "",
                    PatientEmail = x.Patient != null ? x.Patient.Email : "",

                    DoctorID = x.DoctorID,
                    DoctorName = x.Doctor != null ? x.Doctor.DoctorName : "",
                    Specialty = x.Doctor != null ? x.Doctor.Specialty : "",

                    BookingDate = x.BookingDate,
                    AppointmentDate = x.AppointmentDate,

                    IsCancelled = x.IsCancelled,
                    DoctorActive = x.Doctor != null && x.Doctor.Active,
                    DoctorMaxPatients = x.Doctor != null ? x.Doctor.MaxPatients : 0
                })
                .ToListAsync();

            return new AppointmentReportViewModel
            {
                Filter = filter,
                Items = items,
                TotalAppointments = totalAppointments,
                TotalCancelled = totalCancelled,
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalDoctorMaxPatients = totalDoctorMaxPatients
            };

        }
    }
}
