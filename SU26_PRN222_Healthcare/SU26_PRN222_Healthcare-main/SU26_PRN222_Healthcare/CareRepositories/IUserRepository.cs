using SU26_PRN222_Healthcare.CareBusiness.Entities;

namespace SU26_PRN222_Healthcare.CareRepositories
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        User? GetById(int id);
        void Add(User user);

        // Stats
        int CountPatients();
        int CountAdmins();
        IEnumerable<User> GetAllPatients();
    }
}
