using SU26_PRN222_Healthcare.CareBusiness.Entities;

namespace SU26_PRN222_Healthcare.CareRepositories
{
    public interface IDoctorRepository
    {


        IEnumerable<Doctor> GetAll();
        IEnumerable<Doctor> Search(string? name, string? specialty, string? licenseNumber);
        Doctor? GetById(int id);
        void Add(Doctor doctor);
        void Update(Doctor doctor);
        void Delete(int id);
        bool ExistsByLicense(string licenseNumber, int? excludeId = null);

        // Stats
        int CountTotal();
        int CountActive();
        int CountInactive();


    }
}
