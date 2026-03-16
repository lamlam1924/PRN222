using Microsoft.AspNetCore.Mvc;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareDataAccess;
using SU26_PRN222_Healthcare.CareRepositories;
using SU26_PRN222_Healthcare.Models;

namespace SU26_PRN222_Healthcare.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly ISessionRepository _sessionRepo;
        private readonly IUserRepository _userRepo;

        public AppointmentController(
            IAppointmentRepository appointmentRepo,
            IDoctorRepository doctorRepo,
            ISessionRepository sessionRepo,
            IUserRepository userRepo)
        {
            _appointmentRepo = appointmentRepo;
            _doctorRepo = doctorRepo;
            _sessionRepo = sessionRepo;
            _userRepo = userRepo;
        }

        private (bool valid, string role, int userId) ValidateSession()
        {
            var sessionId = HttpContext.Session.GetString("SessionID");
            if (string.IsNullOrEmpty(sessionId)) return (false, "", 0);
            var session = _sessionRepo.GetBySessionId(sessionId);
            if (session == null || session.ExpiresAt <= DateTime.Now) return (false, "", 0);
            return (true, session.Role, session.UserID);
        }

        // ─── PATIENT: Book ───────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Book(int doctorId)
        {
            var (valid, role, _) = ValidateSession();
            if (!valid || role != "Patient") return RedirectToAction("Login", "Account");

            var doctor = _doctorRepo.GetById(doctorId);
            if (doctor == null || !doctor.Active)
            {
                TempData["Error"] = "This doctor is not available for booking.";
                return RedirectToAction("Search", "Doctor");
            }

            ViewBag.Doctor = doctor;
            ViewBag.MinDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        public IActionResult Book(int doctorId, DateTime appointmentDate)
        {
            var (valid, role, userId) = ValidateSession();
            if (!valid || role != "Patient") return RedirectToAction("Login", "Account");

            var doctor = _doctorRepo.GetById(doctorId);
            if (doctor == null) { TempData["Error"] = "Doctor not found."; return RedirectToAction("Search", "Doctor"); }

            ViewBag.Doctor = doctor;
            ViewBag.MinDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            if (!doctor.Active)
            {
                ModelState.AddModelError("", "This doctor is not currently active.");
                return View();
            }

            int bookedCount = _appointmentRepo.CountByDoctorDate(doctorId, appointmentDate);
            if (bookedCount >= doctor.MaxPatients)
            {
                ModelState.AddModelError("", $"This doctor is fully booked for {appointmentDate:dd/MM/yyyy}. Maximum {doctor.MaxPatients} patients per day.");
                return View();
            }

            if (_appointmentRepo.HasPatientBooked(userId, doctorId, appointmentDate))
            {
                ModelState.AddModelError("", $"You have already booked Dr. {doctor.DoctorName} on {appointmentDate:dd/MM/yyyy}.");
                return View();
            }

            _appointmentRepo.Add(new Appointment
            {
                PatientID = userId,
                DoctorID = doctorId,
                BookingDate = DateTime.Now,
                AppointmentDate = appointmentDate,
                IsCancelled = false
            });

            TempData["Success"] = $"Appointment with Dr. {doctor.DoctorName} booked for {appointmentDate:dd/MM/yyyy}.";
            return RedirectToAction("MyAppointments");
        }

        // ─── PATIENT: My Appointments (all, including cancelled) ─────────────────────

        public IActionResult MyAppointments(string? filter)
        {
            var (valid, role, userId) = ValidateSession();
            if (!valid || role != "Patient") return RedirectToAction("Login", "Account");

            IEnumerable<Appointment> appointments;
            if (filter == "upcoming")
                appointments = _appointmentRepo.GetUpcomingByPatient(userId);
            else if (filter == "past")
                appointments = _appointmentRepo.GetPastByPatient(userId);
            else
                appointments = _appointmentRepo.GetByPatient(userId);

            ViewBag.Filter = filter ?? "all";
            return View(appointments.ToList());
        }

        // ─── PATIENT: Cancel own appointment ─────────────────────────────────────────

        [HttpPost]
        public IActionResult CancelOwn(int id)
        {
            var (valid, role, userId) = ValidateSession();
            if (!valid || role != "Patient") return RedirectToAction("Login", "Account");

            var appt = _appointmentRepo.GetById(id);
            if (appt == null || appt.PatientID != userId)
            {
                TempData["Error"] = "Appointment not found or access denied.";
                return RedirectToAction("MyAppointments");
            }
            if (appt.IsCancelled)
            {
                TempData["Error"] = "Appointment is already cancelled.";
                return RedirectToAction("MyAppointments");
            }
            if (appt.AppointmentDate <= DateTime.Now)
            {
                TempData["Error"] = "Cannot cancel a past appointment.";
                return RedirectToAction("MyAppointments");
            }

            _appointmentRepo.Cancel(id);
            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToAction("MyAppointments");
        }

        // ─── ADMIN: View all appointments with filters ────────────────────────────────

        public IActionResult AllAppointments(DateTime? date, int? doctorId, int? patientId)
        {
            var (valid, role, _) = ValidateSession();
            if (!valid || role != "Admin") return RedirectToAction("Login", "Account");

            var appointments = _appointmentRepo.GetAll(date, doctorId, patientId).ToList();
            ViewBag.Doctors = _doctorRepo.GetAll().ToList();
            ViewBag.Patients = _userRepo.GetAllPatients().ToList();
            ViewBag.FilterDate = date?.ToString("yyyy-MM-dd");
            ViewBag.FilterDoctorId = doctorId;
            ViewBag.FilterPatientId = patientId;
            return View(appointments);
        }

        // ─── ADMIN: Cancel any appointment ───────────────────────────────────────────

        [HttpPost]
        public IActionResult CancelAdmin(int id)
        {
            var (valid, role, _) = ValidateSession();
            if (!valid || role != "Admin") return RedirectToAction("Login", "Account");

            var appt = _appointmentRepo.GetById(id);
            if (appt == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction("AllAppointments");
            }

            _appointmentRepo.Cancel(id);
            TempData["Success"] = $"Appointment #{id} cancelled.";
            return RedirectToAction("AllAppointments");
        }

        // ─── ADMIN: Doctor slot capacity for a date ───────────────────────────────────

        public IActionResult DoctorSlots(DateTime? date)
        {
            var (valid, role, _) = ValidateSession();
            if (!valid || role != "Admin") return RedirectToAction("Login", "Account");

            date ??= DateTime.Today;
            var doctors = _doctorRepo.GetAll().ToList();
            var slotData = doctors.Select(d =>
            {
                int booked = _appointmentRepo.CountByDoctorDate(d.ID, date.Value);
                return new SU26_PRN222_Healthcare.Models.DoctorSlotViewModel
                {
                    Doctor = d,
                    Booked = booked,
                    Remaining = Math.Max(0, d.MaxPatients - booked)
                };
            }).ToList();

            ViewBag.SelectedDate = date.Value.ToString("yyyy-MM-dd");
            ViewBag.SelectedDateDisplay = date.Value.ToString("dd/MM/yyyy");
            return View(slotData);
        }


        [HttpGet]
        public async Task<IActionResult> Report(AppointmentFilterViewModel filter)
        {
            // CHỖ NHẬN DỮ LIỆU TỪ VIEW:
            // filter sẽ tự bind từ query string/form get

            var vm = await _appointmentRepo.GetReportAsync(filter);

            // CHỖ ĐỔ DỮ LIỆU SANG VIEW
            return View(vm);
        }
    }
}
