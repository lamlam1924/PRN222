using SU26_PRN222_Healthcare.CareBusiness.Entities;

namespace SU26_PRN222_Healthcare.CareRepositories
{
    public interface ISessionRepository
    {
        void Create(Session session);
        Session? GetBySessionId(string sessionId);
        void Delete(string sessionId);
    }
}
