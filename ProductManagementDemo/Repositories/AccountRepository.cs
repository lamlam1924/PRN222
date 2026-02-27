using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public AccountMember? GetAccountById(string memberId) 
            => AccountDAO.Instance.GetAccountById(memberId);

        public AccountMember? CheckLogin(string email, string password) 
            => AccountDAO.Instance.CheckLogin(email, password);
    }
}
