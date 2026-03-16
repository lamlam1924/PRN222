using SU26_PRN222_Healthcare.CareBusiness;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;

namespace SU26_PRN222_Healthcare.CareDataAccess
{
    public class SessionRepository : ISessionRepository
    {
        private readonly CareDbContext _context;

        public SessionRepository(CareDbContext context)
        {
            _context = context;
        }

        public void Create(Session session)
        {
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }

        public Session? GetBySessionId(string sessionId)
        {
            return _context.Sessions
                .FirstOrDefault(s => s.SessionID == sessionId);
        }

        public void Delete(string sessionId)
        {
            var session = _context.Sessions.Find(sessionId);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                _context.SaveChanges();
            }
        }
    }
}
