using Microsoft.AspNetCore.Mvc;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;

namespace SU26_PRN222_Healthcare.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly ISessionRepository _sessionRepo;

        public DoctorController(IDoctorRepository doctorRepo, ISessionRepository sessionRepo)
        {
            _doctorRepo = doctorRepo;
            _sessionRepo = sessionRepo;
        }

        // Helper: validate session and get role
        private string? GetCurrentRole()
        {
            var sessionId = HttpContext.Session.GetString("SessionID");
            if (string.IsNullOrEmpty(sessionId)) return null;

            var session = _sessionRepo.GetBySessionId(sessionId);
            if (session == null || session.ExpiresAt <= DateTime.Now) return null;

            return session.Role;
        }

        // GET: /Doctor/Index  (Admin only - list all)
        public IActionResult Index()
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var doctors = _doctorRepo.GetAll().ToList();
            return View(doctors);
        }

        // GET: /Doctor/Search  (Patient - search)
        public IActionResult Search(string? name, string? specialty, string? licenseNumber)
        {
            var role = GetCurrentRole();
            if (role == null) return RedirectToAction("Login", "Account");

            bool searched = !string.IsNullOrWhiteSpace(name)
                         || !string.IsNullOrWhiteSpace(specialty)
                         || !string.IsNullOrWhiteSpace(licenseNumber);

            ViewBag.Searched = searched;
            ViewBag.SearchName = name;
            ViewBag.SearchSpecialty = specialty;
            ViewBag.SearchLicense = licenseNumber;

            if (searched)
            {
                var results = _doctorRepo.Search(name, specialty, licenseNumber).ToList();
                return View(results);
            }

            return View(new List<Doctor>());
        }

        // GET: /Doctor/Create  (Admin only)
        [HttpGet]
        public IActionResult Create()
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: /Doctor/Create
        [HttpPost]
        public IActionResult Create(Doctor doctor)
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");

            // Validation
            if (string.IsNullOrWhiteSpace(doctor.DoctorName))
                ModelState.AddModelError("DoctorName", "Doctor name is required.");

            if (string.IsNullOrWhiteSpace(doctor.Specialty))
                ModelState.AddModelError("Specialty", "Specialty is required.");

            if (string.IsNullOrWhiteSpace(doctor.LicenseNumber))
                ModelState.AddModelError("LicenseNumber", "License number is required.");

            if (doctor.MaxPatients <= 0)
                ModelState.AddModelError("MaxPatients", "Max patients must be > 0.");

            if (!string.IsNullOrWhiteSpace(doctor.LicenseNumber) && _doctorRepo.ExistsByLicense(doctor.LicenseNumber))
                ModelState.AddModelError("LicenseNumber", "A doctor with this license number already exists.");

            if (!ModelState.IsValid)
                return View(doctor);

            _doctorRepo.Add(doctor);
            TempData["Success"] = $"Doctor '{doctor.DoctorName}' added successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Doctor/Edit/{id}  (Admin only)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var doctor = _doctorRepo.GetById(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        // POST: /Doctor/Edit
        [HttpPost]
        public IActionResult Edit(Doctor doctor)
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(doctor.DoctorName))
                ModelState.AddModelError("DoctorName", "Doctor name is required.");

            if (string.IsNullOrWhiteSpace(doctor.Specialty))
                ModelState.AddModelError("Specialty", "Specialty is required.");

            if (string.IsNullOrWhiteSpace(doctor.LicenseNumber))
                ModelState.AddModelError("LicenseNumber", "License number is required.");

            if (doctor.MaxPatients < 0)
                ModelState.AddModelError("MaxPatients", "Max patients must be >= 0.");

            if (!string.IsNullOrWhiteSpace(doctor.LicenseNumber) && _doctorRepo.ExistsByLicense(doctor.LicenseNumber, doctor.ID))
                ModelState.AddModelError("LicenseNumber", "A doctor with this license number already exists.");

            if (!ModelState.IsValid)
                return View(doctor);

            _doctorRepo.Update(doctor);
            TempData["Success"] = $"Doctor '{doctor.DoctorName}' updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Doctor/Delete/{id}  (Admin only)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var role = GetCurrentRole();
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var doctor = _doctorRepo.GetById(id);
            if (doctor != null)
            {
                _doctorRepo.Delete(id);
                TempData["Success"] = $"Doctor '{doctor.DoctorName}' deleted successfully.";
            }
            return RedirectToAction("Index");
        }


    }
}
