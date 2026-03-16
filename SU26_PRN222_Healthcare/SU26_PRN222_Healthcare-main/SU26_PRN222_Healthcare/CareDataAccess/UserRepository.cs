using Microsoft.EntityFrameworkCore;
using SU26_PRN222_Healthcare.CareBusiness;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;

namespace SU26_PRN222_Healthcare.CareDataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly CareDbContext _context;

        public UserRepository(CareDbContext context)
        {
            _context = context;
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.ID == id);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public int CountPatients() => _context.Users.Count(u => u.Role == "Patient");
        public int CountAdmins() => _context.Users.Count(u => u.Role == "Admin");

        public IEnumerable<User> GetAllPatients()
        {
            return _context.Users.Where(u => u.Role == "Patient").ToList();
        }
    }
}
