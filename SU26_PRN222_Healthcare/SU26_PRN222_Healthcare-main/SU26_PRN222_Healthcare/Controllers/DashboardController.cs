using Microsoft.AspNetCore.Mvc;
using SU26_PRN222_Healthcare.CareRepositories;
using SU26_PRN222_Healthcare.Models;

namespace SU26_PRN222_Healthcare.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUserRepository _userRepo;
        private readonly ISessionRepository _sessionRepo;

        public DashboardController(
            IAppointmentRepository appointmentRepo,
            IDoctorRepository doctorRepo,
            IUserRepository userRepo,
            ISessionRepository sessionRepo)
        {
            _appointmentRepo = appointmentRepo;
            _doctorRepo = doctorRepo;
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
        }

        private bool IsAdmin()
        {
            var sessionId = HttpContext.Session.GetString("SessionID");
            if (string.IsNullOrEmpty(sessionId)) return false;
            var session = _sessionRepo.GetBySessionId(sessionId);
            return session != null && session.ExpiresAt > DateTime.Now && session.Role == "Admin";
        }

        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var vm = new DashboardStatsViewModel
            {
                TotalDoctors        = _doctorRepo.CountTotal(),
                ActiveDoctors       = _doctorRepo.CountActive(),
                InactiveDoctors     = _doctorRepo.CountInactive(),
                TotalPatients       = _userRepo.CountPatients(),
                TotalAppointments   = _appointmentRepo.CountTotal(),
                ActiveAppointments  = _appointmentRepo.CountActive(),
                CancelledAppointments = _appointmentRepo.CountCancelled(),
                TodayAppointments   = _appointmentRepo.CountToday(),
                StatsByDoctor       = _appointmentRepo.GetStatsByDoctor().ToList(),
                StatsBySpecialty    = _appointmentRepo.GetStatsBySpecialty().ToList()
            };

            return View(vm);
        }
    }
}
