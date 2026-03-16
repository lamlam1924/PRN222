using Microsoft.EntityFrameworkCore;
using SU26_PRN222_Healthcare.CareBusiness;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;

namespace SU26_PRN222_Healthcare.CareDataAccess
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly CareDbContext _context;

        public DoctorRepository(CareDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Doctor> GetAll()
        {
            return _context.Doctors.ToList();
        }

        public IEnumerable<Doctor> Search(string? name, string? specialty, string? licenseNumber)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.DoctorName.Contains(name));

            if (!string.IsNullOrWhiteSpace(specialty))
                query = query.Where(d => d.Specialty.Contains(specialty));

            if (!string.IsNullOrWhiteSpace(licenseNumber))
                query = query.Where(d => d.LicenseNumber.Contains(licenseNumber));

            return query.ToList();
        }

        public Doctor? GetById(int id)
        {
            return _context.Doctors.FirstOrDefault(d => d.ID == id);
        }

        public void Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        public void Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
            }
        }

        public bool ExistsByLicense(string licenseNumber, int? excludeId = null)
        {
            var query = _context.Doctors.AsQueryable();
            query = query.Where(d => d.LicenseNumber == licenseNumber);
            if (excludeId.HasValue)
                query = query.Where(d => d.ID != excludeId.Value);
            return query.Any();
        }

        // Stats
        public int CountTotal() => _context.Doctors.Count();
        public int CountActive() => _context.Doctors.Count(d => d.Active);
        public int CountInactive() => _context.Doctors.Count(d => !d.Active);


       public void GetTop3(Doctor[] doctors, DateTime? dateTime)
       {
            //list top 3 doctor have the most appointment at dateTime
            
       }
    }
}
